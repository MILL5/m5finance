using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    // http://www.nasdaqtrader.com/dynamic/SymDir/nasdaqlisted.txt
    // Symbol|Security Name|Market Category|Test Issue|Financial Status|Round Lot Size|ETF|NextShares

    public class NTNasdaqClient : ISecurityClient
    {
        private const string NASDAQ_MICCODE = "XNAS";

        private const string SYMBOL_FIELD = "Symbol";
        private const string NAME_FIELD = "Security Name";
        private const string TEST_FIELD = "Test Issue";

        private const string SECURITIES_URL = "http://www.nasdaqtrader.com/dynamic/SymDir/nasdaqlisted.txt";

        private readonly HttpClient _client;
        private readonly Exchange _exchange;

        public NTNasdaqClient()
        {
            _client = HttpInternal.Client;
            var exchangeService = new ExchangeService();
            _exchange = exchangeService.GetExchangeByMicCodeAsync(NASDAQ_MICCODE).Result;

            CheckIsNotNull("exchange", _exchange);
        }

        public NTNasdaqClient(HttpClient client, IExchangeService exchangeService)
        {
            CheckIsNotNull(nameof(client), client);
            CheckIsNotNull(nameof(exchangeService), exchangeService);

            _client = client;

            _exchange = exchangeService.GetExchangeByMicCodeAsync(NASDAQ_MICCODE).Result;

            CheckIsNotNull("exchange", _exchange);
        }

        public async Task<IEnumerable<Security>> GetSecuritiesAsync()
        {
            var securities = await _client.DownloadFileAsStringAsync(SECURITIES_URL);

            var listOfSecurities = new List<Security>(2400);

            using (var textReader = new StringReader(securities))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    Delimiter = "|"
                };

                using (var csvReader = new CsvReader(textReader, config))
                {
                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    while (await csvReader.ReadAsync())
                    {
                        var test = csvReader.GetField(TEST_FIELD);

                        if (!test.StartsWith("Y"))
                        {
                            var s = new Security
                            {
                                Ticker = csvReader.GetField(SYMBOL_FIELD),
                                Name = csvReader.GetField(NAME_FIELD),
                                ExchangeMic = _exchange.Mic
                            };

                            listOfSecurities.Add(s);
                        }
                    }
                }
            }

            return listOfSecurities;
        }

        public Task<IEnumerable<NasdaqSecurityExchange>> GetSecuritiesByExchangeAsync(string exchangeMic, string exchangeAcronym)
        {
            throw new NotImplementedException();
        }
    }
}
