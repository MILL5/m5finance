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
        }

        public OpenFigiClient(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);
            _client = client;
        }

        /// <summary>
        /// Brings the OpenFigi data for mapping security identifiers
        /// </summary>
        /// <param name="openFigiRequestList">List of parameters</param>
        /// <returns>List of opening figi instruments.</returns>
        public async Task<IEnumerable<OpenFigiInstrument>> GetFigiMappingsAsync(IEnumerable<OpenFigiRequest> openFigiRequestList)
        {
            CheckIsNotNull(nameof(openFigiRequestList), openFigiRequestList.Count());
            CheckIsNotLessThanOrEqualTo(nameof(openFigiRequestList), openFigiRequestList.Count(), 0);
            CheckIsWellFormedUri(nameof(OPENFIGI_SECURITIES_URL), OPENFIGI_SECURITIES_URL, UriKind.Absolute);
            
            _client.DefaultRequestHeaders.Add("X-OPENFIGI-APIKEY", Environment.GetEnvironmentVariable("OpenFigiApiKey"));

            var response = await _client.SendAsStringAsync(OPENFIGI_SECURITIES_URL, JsonConvert.SerializeObject(openFigiRequestList, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            var result = JsonConvert.DeserializeObject<List<OpenFIGIArrayResponse>>(response);

            if (result.First().Error == null || !result.First().Error.Any())
            {
                // ToDo: Pending to cover more than one data array
                return result.First().Data;
            }
            else
            {
                throw new Exception(result.First().Error);
            }
        }
    }
}
