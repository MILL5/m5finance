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
    // https://api.nasdaq.com/api/screener/stocks?download=true

    public class NASDAQClient : ISecurityClient
    {
        private const string NASDAQ_SECURITIES_URL = "https://api.nasdaq.com/api/screener/stocks?download=true";
        private const string NASDAQ_MICCODE = "YNAS";
        private const string EXCHANGE_QUERY = "&exchange=";

        private readonly HttpClient _client;

        public NASDAQClient()
        {
            _client = HttpInternal.Client;
        }

        public NASDAQClient(HttpClient client)
        {
            CheckIsNotNull(nameof(client), client);
            _client = client;
        }

        public async Task<IEnumerable<Security>> GetSecuritiesAsync()
        {
            var file = await _client.DownloadFileAsStringAsync(NASDAQ_SECURITIES_URL);

            var listOfSecurities = new List<Security>(10000);

            foreach (var r in NasdaqSecurities.FromJson(file).Data.Rows)
            {
                var s = new Security()
                {
                    Name = r.Name,
                    Ticker = r.Symbol,
                    ExchangeMic = NASDAQ_MICCODE
                };

                listOfSecurities.Add(s);
            }

            return listOfSecurities;
        }

        public async Task<IEnumerable<NasdaqSecuritiesExchange>> GetSecuritiesByExchangeAsync(string exchangeMic, string exchangeAcronym)
        {
            var exchangeUrl = string.Concat(NASDAQ_SECURITIES_URL, EXCHANGE_QUERY, exchangeAcronym);

            var file = await _client.DownloadFileAsStringAsync(exchangeUrl);

            var listOfSecurities = new List<NasdaqSecuritiesExchange>(10000);

            foreach (var row in NasdaqSecurities.FromJson(file).Data.Rows)
            {
                var securityExchange = new NasdaqSecuritiesExchange(row, exchangeMic, exchangeAcronym);

                listOfSecurities.Add(securityExchange);
            }

            return listOfSecurities;
        }
    }
}
