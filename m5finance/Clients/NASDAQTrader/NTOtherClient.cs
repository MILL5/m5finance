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
    // http://www.nasdaqtrader.com/dynamic/SymDir/otherlisted.txt
    // ACT Symbol|Security Name|Exchange|CQS Symbol|ETF|Round Lot Size|Test Issue|NASDAQ Symbol

    public class NTOtherClient : ISecurityClient
    {
        private const string NYSE_MICCODE = "XNYS";
        private const string NYSE_MKT_MICCODE = "XASE";
        private const string NYSE_ARCA_MICCODE = "ARCX";
        private const string BATS_GLOBAL_MICCODE = "XETR";
        private const string INVESTOR_EXCH_MICCODE = "IEXG";

        private const string SYMBOL_FIELD = "ACT Symbol";
        private const string NAME_FIELD = "Security Name";
        private const string EXCHANGE_FIELD = "Exchange";
        private const string TEST_FIELD = "Test Issue";

        private const string SECURITIES_URL = "http://www.nasdaqtrader.com/dynamic/SymDir/otherlisted.txt";

        private readonly HttpClient _client;
        private readonly IDictionary<string, Exchange> _exchanges;

        public NTOtherClient()
        {
            _client = HttpInternal.Client;

            var exchangeService = new ExchangeService();

            _exchanges = new Dictionary<string, Exchange>();

            Initialize(exchangeService);
        }

        public NTOtherClient(HttpClient client, IExchangeService exchangeService)
        {
            CheckIsNotNull(nameof(client), client);
            CheckIsNotNull(nameof(exchangeService), exchangeService);

            _client = client;

            _exchanges = new Dictionary<string, Exchange>();

            Initialize(exchangeService);
        }

        private void Initialize(IExchangeService exchangeService)
        {
            // https://www.nasdaqtrader.com/Trader.aspx?id=SymbolDirDefs
            //
            // A = NYSE MKT
            // N = New York Stock Exchange(NYSE)
            // P = NYSE ARCA
            // Z = BATS Global Markets(BATS)
            // V = Investors' Exchange, LLC (IEXG)
            const string NYSE_MKT_KEY = "A";
            const string NYSE_KEY = "N";
            const string NYSE_ARCA_KEY = "P";
            const string BATS_GLOBAL_KEY = "Z";
            const string INVESTOR_EXCHANGE_KEY = "V";

            var exchange = exchangeService.GetExchangeByMicCodeAsync(NYSE_MICCODE).Result;
            _exchanges.Add(NYSE_KEY, exchange);

            exchange = exchangeService.GetExchangeByMicCodeAsync(NYSE_MKT_MICCODE).Result;
            _exchanges.Add(NYSE_MKT_KEY, exchange);

            exchange = exchangeService.GetExchangeByMicCodeAsync(NYSE_ARCA_MICCODE).Result;
            _exchanges.Add(NYSE_ARCA_KEY, exchange);

            exchange = exchangeService.GetExchangeByMicCodeAsync(BATS_GLOBAL_MICCODE).Result;
            _exchanges.Add(BATS_GLOBAL_KEY, exchange);

            exchange = exchangeService.GetExchangeByMicCodeAsync(INVESTOR_EXCH_MICCODE).Result;
            _exchanges.Add(INVESTOR_EXCHANGE_KEY, exchange);
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
                            var exchangeKey = csvReader.GetField(EXCHANGE_FIELD);

                            if (!string.IsNullOrWhiteSpace(exchangeKey))
                            {
                                var exchange = _exchanges[exchangeKey];

                                if (exchange != null)
                                {
                                    var s = new Security
                                    {
                                        Ticker = csvReader.GetField(SYMBOL_FIELD),
                                        Name = csvReader.GetField(NAME_FIELD),
                                        ExchangeMic = exchange.Mic,
                                    };

                                    listOfSecurities.Add(s);
                                }
                            }
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
