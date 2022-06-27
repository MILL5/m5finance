using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public class OpenFigiClient : IOpenFigiClient
    {
        private static readonly Uri OpenFigiMappingV1Url = new Uri("https://api.openfigi.com/v1/mapping");
        private static readonly Uri OpenFigiFilterV3Url = new Uri("https://api.openfigi.com/v3/filter");
        private static readonly IOpenFigiLimits _limitsWithKey = new OpenFigiLimitWithApiKey();
        private static readonly IOpenFigiLimits _filterLimitsWithKey = new OpenFigiFilterLimitWithApiKey();

        private const string OPENFIGI_API_KEY_HEADER_NAME = "X-OPENFIGI-APIKEY";

        private readonly HttpClient _client;
        private readonly IOpenFigiLimits _limits;

        public IOpenFigiLimits CurrentLimits => _limits;

        public OpenFigiClient(string openFigiApiKey)
        {
            CheckIsNotNullOrWhitespace(nameof(openFigiApiKey), openFigiApiKey);
            _limits = _limitsWithKey;
            _client = HttpInternal.Client;
            _client.DefaultRequestHeaders.Add(OPENFIGI_API_KEY_HEADER_NAME, openFigiApiKey);
        }

        /// <summary>
        /// Brings the OpenFigi data for mapping security identifiers
        /// </summary>
        /// <param name="openFigiRequestList">List of parameters</param>
        /// <returns>List of opening figi instruments.</returns>
        public async Task<IEnumerable<OpenFigiInstrument>> GetFigiMappingsAsync(IEnumerable<OpenFigiRequest> openFigiRequestList)
        {
            CheckIsNotNull(nameof(openFigiRequestList), openFigiRequestList);
            CheckIsNotLessThanOrEqualTo(nameof(openFigiRequestList), openFigiRequestList.Count(), 0);
            _limits.CheckJobLimit(nameof(openFigiRequestList), openFigiRequestList);

            IEnumerable<OpenFigiArrayResponse> response;

            using (await _limits.ApiLimiter.GetOperationScopeAsync())
            {
                response = await _client.SendAsync<IEnumerable<OpenFigiRequest>, IEnumerable<OpenFigiArrayResponse>>(OpenFigiMappingV1Url.ToString(), openFigiRequestList);
            }

            var result = new List<OpenFigiInstrument>();
            foreach (var instruments in response.Where(r => r.Data != null))
            {
                result.AddRange(instruments.Data);
            }

            return result;
        }

        /// <summary>
        /// Gets the OpenFigi data filtered by exchange code
        /// </summary>
        /// <param exchangeCode="exchangeCode">The exchange code</param>
        /// <returns>The OpenFigi response</returns>
        public async Task<OpenFigiResponseV3> GetFigiMappingsForExchangeAsync(
            string exchangeCode,
            string marketSector = OpenFigiConsts.OpenFigiMarketSector.Equity,
            string securityType = OpenFigiConsts.OpenFigiSecurityTypes.CommonStock,
            string next = null)
        {
            CheckIsNotNullOrWhitespace(nameof(exchangeCode), exchangeCode);

            var result = new List<OpenFigiInstrumentV3>();

            var request = new OpenFigiRequestV3()
            {
                ExchCode = exchangeCode,
                MarketSectorDesc = marketSector,
                SecurityType = securityType,
                Start = next
            };

            OpenFigiResponseV3 response;
            using (await _filterLimitsWithKey.ApiLimiter.GetOperationScopeAsync())
            {
                response = await _client.SendAsync<OpenFigiRequestV3, OpenFigiResponseV3>(OpenFigiFilterV3Url.ToString(), request);
                result.AddRange(response.Data);
                request.Start = response.Next;
            }

            return response;
        }
    }
}