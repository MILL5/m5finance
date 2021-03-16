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
    // https://raw.githubusercontent.com/janlukasschroeder/cik-mapping/master/assets/cik_ticker.csv
    // CIK|Ticker|Name|Exchange|SIC|Business|Incorporated|IRS
    // 1090872|A|Agilent Technologies Inc|NYSE|3825|CA|DE|770518772

    public class GitHubCIKMappingClient
    {
        private const string SEC_CIK_TO_TICKER_URL = " https://raw.githubusercontent.com/janlukasschroeder/cik-mapping/master/assets/cik_ticker.csv";

        private const string CIK_FIELD = "CIK";
        private const string TICKER_FIELD = "Ticker";

        private readonly HttpClient _client;

        public GitHubCIKMappingClient()
        {
            _client = HttpInternal.Client;
        }

        public GitHubCIKMappingClient(HttpClient client)
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
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    Delimiter = $"|"
                };

                using (var csvReader = new CsvReader(textReader, config))
                {
                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    while (await csvReader.ReadAsync())
                    {
                        var m = new CikMapping
                        {
                            Ticker = csvReader.GetField<string>(TICKER_FIELD).ToUpperInvariant(),
                            CIK = csvReader.GetField<int>(CIK_FIELD)
                        };

                        listOfMappings.Add(m);
                    }
                }
            }

            return new CikLookup(listOfMappings);
        }
    }
}
