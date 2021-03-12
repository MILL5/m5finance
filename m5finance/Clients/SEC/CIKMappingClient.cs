using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    // https://www.sec.gov/include/ticker.txt
    // aapl	320193

    public class CIKMappingClient
    {
        private const string SEC_CIK_TO_TICKER_URL = "https://www.sec.gov/include/ticker.txt";

        private readonly HttpClient _client;

        public CIKMappingClient()
        {
            _client = HttpInternal.Client;
        }

        public CIKMappingClient(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);

            _client = client;
        }

        public async Task<CikLookup> GetCikMappingsAsync()
        {
            var securities = await _client.DownloadFileAsStringAsync(SEC_CIK_TO_TICKER_URL);

            var listOfMappings = new List<CikMapping>(12000);

            using (var textReader = new StringReader(securities))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    Delimiter = $"\t"
                };


                using (var csvReader = new CsvReader(textReader, config))
                {
                    await csvReader.ReadAsync();

                    while (await csvReader.ReadAsync())
                    {
                        var m = new CikMapping
                        {
                            Ticker = csvReader.GetField<string>(0).ToUpperInvariant(),
                            CIK = csvReader.GetField<int>(1)
                        };

                        listOfMappings.Add(m);
                    }
                }
            }

            return new CikLookup(listOfMappings);
        }
    }
}
