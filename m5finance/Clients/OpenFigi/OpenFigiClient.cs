using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public class OpenFigiClient : IOpenFigiClient
    {
        private const string OPENFIGI_SECURITIES_URL = "https://api.openfigi.com/v1/mapping";
        private const string OPENFIGI_API_KEY_HEADER_NAME = "X-OPENFIGI-APIKEY";

        private const string OPENFIGI_V3_FILTER_URL = "https://api.openfigi.com/v3/filter";

        private readonly HttpClient _client;

        private static readonly IOpenFigiLimits _limitsWithKey;
        private static readonly IOpenFigiLimits _limitsWithoutKey;
        private static readonly IOpenFigiLimits _filterLimitsWithKey;

        private readonly IOpenFigiLimits _limits;

        public IOpenFigiLimits CurrentLimits => _limits;

        static OpenFigiClient()
        {
            _limitsWithKey = new OpenFigiLimitWithApiKey();
            _limitsWithoutKey = new OpenFigiLimitWithOutApiKey();

            _filterLimitsWithKey = new OpenFigiFilterLimitWithApiKey();
        }

        public OpenFigiClient()
        {
            _limits = _limitsWithoutKey;
        }

        public OpenFigiClient(string apiKey)
        {
            CheckIsNotNullOrWhitespace(nameof(apiKey), apiKey);
            _limits = _limitsWithKey;
            _client = HttpInternal.Client;
            _client.DefaultRequestHeaders.Add(OPENFIGI_API_KEY_HEADER_NAME, apiKey);
        }

        public OpenFigiClient(HttpClient client, string apiKey)
        {
            CheckIsNotNull(nameof(client), client);
            CheckIsNotNullOrWhitespace(nameof(apiKey), apiKey);
            _limits = _limitsWithKey;
            _client = client;
            _client.DefaultRequestHeaders.Add(OPENFIGI_API_KEY_HEADER_NAME, apiKey);
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
                response = await _client.SendAsync<IEnumerable<OpenFigiRequest>, IEnumerable<OpenFigiArrayResponse>>(OPENFIGI_SECURITIES_URL, openFigiRequestList);
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
        public async Task<OpenFigiResponseV3> GetFigiMappingsForExchangeAsync(string exchangeCode, string next = null)
        {
            CheckIsNotNullOrWhitespace(nameof(exchangeCode), exchangeCode);

            var result = new List<OpenFigiInstrumentV3>();

            var request = new OpenFigiRequestV3() 
            { 
                ExchCode = exchangeCode,
                MarketSectorDesc = "Equity",
                SecurityType = "Common Stock",
                Start = next
            };

            OpenFigiResponseV3 response;
            using (await _filterLimitsWithKey.ApiLimiter.GetOperationScopeAsync())
            {
                response = await _client.SendAsync<OpenFigiRequestV3, OpenFigiResponseV3>(OPENFIGI_V3_FILTER_URL, request);
                result.AddRange(response.Data);
                request.Start = response.Next;
            }

            return response;
        }
    }
}