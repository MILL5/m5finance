using Newtonsoft.Json;
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
        private const string OPENFIGI_SECURITIES_URL = "https://api.openfigi.com/v1/mapping";
        private const string OPENFIGI_API_KEY_HEADER_NAME = "X-OPENFIGI-APIKEY";

        private readonly HttpClient _client;

        public OpenFigiClient(string apiKey)
        {
            CheckIsNotNullOrWhitespace(nameof(apiKey), apiKey);
            _client = HttpInternal.Client;
            _client.DefaultRequestHeaders.Add(OPENFIGI_API_KEY_HEADER_NAME, apiKey);
        }

        public OpenFigiClient(HttpClient client, string apiKey)
        {
            CheckIsNotNull(nameof(client), client);
            CheckIsNotNullOrWhitespace(nameof(apiKey), apiKey);
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
            CheckIsWellFormedUri(nameof(OPENFIGI_SECURITIES_URL), OPENFIGI_SECURITIES_URL, UriKind.Absolute);

            var response = await _client.SendAsStringAsync(OPENFIGI_SECURITIES_URL, JsonConvert.SerializeObject(openFigiRequestList, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            var resultList = JsonConvert.DeserializeObject<List<OpenFIGIArrayResponse>>(response);

            var openFigiInstrumentsResponse = new List<OpenFigiInstrument>();
            foreach (var result in resultList.Where(r => r.Data != null))
            {
                openFigiInstrumentsResponse.AddRange(result.Data);
            }

            return openFigiInstrumentsResponse;
        }
    }
}
