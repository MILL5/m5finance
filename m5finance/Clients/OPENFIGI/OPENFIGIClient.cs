using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using static System.Environment;

namespace M5Finance
{
    public class OpenFigiClient 
    {
        private const string OPENFIGI_SECURITIES_URL = "https://api.openfigi.com/v1/mapping";

        private readonly HttpClient _client;

        public OpenFigiClient()
        {
            _client = HttpInternal.Client;
            _client.DefaultRequestHeaders.Add("X-OPENFIGI-APIKEY", Environment.GetEnvironmentVariable("OpenFigiApiKey"));
        }

        public OpenFigiClient(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);
            _client = client;
            _client.DefaultRequestHeaders.Add("X-OPENFIGI-APIKEY", Environment.GetEnvironmentVariable("OpenFigiApiKey"));
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
