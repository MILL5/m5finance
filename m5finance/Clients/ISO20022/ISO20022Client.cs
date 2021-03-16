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
    // https://www.iso20022.org/sites/default/files/ISO10383_MIC/ISO10383_MIC.csv
    // "COUNTRY","ISO COUNTRY CODE (ISO 3166)","MIC","OPERATING MIC","O/S","NAME-INSTITUTION DESCRIPTION","ACRONYM","CITY","WEBSITE","STATUS DATE","STATUS","CREATION DATE","COMMENTS"

    public class ISO20022Client
    {
        private const string COUNTRY_FIELD = "COUNTRY";
        private const string ISO_COUNTRY_CODE_FIELD = "ISO COUNTRY CODE (ISO 3166)";
        private const string MIC_FIELD = "MIC";
        private const string OPERATING_MIC_FIELD = "OPERATING MIC";
        private const string NAME_FIELD = "NAME-INSTITUTION DESCRIPTION";
        private const string ACRONYM_FIELD = "ACRONYM";
        private const string WEBSITE_FIELD = "WEBSITE";

        private const string EXCHANGES_URL = "https://www.iso20022.org/sites/default/files/ISO10383_MIC/ISO10383_MIC.csv";

        private readonly HttpClient _client;

        public ISO20022Client()
        {
            _client = HttpInternal.Client;
        }

        public ISO20022Client(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);
            _client = client;
        }

        public async Task<IEnumerable<Exchange>> GetExchangesAsync()
        {
            var exchanges = await _client.DownloadFileAsStringAsync(EXCHANGES_URL);

            var listOfExchanges = new List<Exchange>(2400);

            using (var textReader = new StringReader(exchanges))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    Delimiter = ","
                };

                using (var csvReader = new CsvReader(textReader, config))
                {
                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    while (await csvReader.ReadAsync())
                    {
                        var e = new Exchange
                        {
                            Country = csvReader.GetField(COUNTRY_FIELD),
                            IsoCountryCode =  csvReader.GetField(ISO_COUNTRY_CODE_FIELD),
                            Mic = csvReader.GetField(MIC_FIELD),
                            OperatingMic = csvReader.GetField(OPERATING_MIC_FIELD),
                            Name = csvReader.GetField(NAME_FIELD),
                            Acronym = csvReader.GetField(ACRONYM_FIELD),
                            Website = csvReader.GetField(WEBSITE_FIELD),
                        };

                        listOfExchanges.Add(e);
                    }
                }
            }

            return listOfExchanges;
        }
    }
}
