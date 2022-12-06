using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace M5Finance.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NasdaqTests
    {
        private readonly NASDAQClient _client;

        public NasdaqTests()
        {
            _client = new NASDAQClient();
        }

        [TestMethod]
        public async Task GetNasdaqSecuritiesTestAsync()
        {
            var securities = await _client.GetSecuritiesAsync();

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Any());
        }

        [DataTestMethod]
        [DataRow("XNAS", "NASDAQ")]
        [DataRow("XNYS", "NYSE")]
        [DataRow("XASE", "AMEX")]
        public async Task GetSecuritiesByExchangeTestAsync(string exchangeMic, string exchangeAcronym)
        {
            var securities = await _client.GetSecuritiesByExchangeAsync(exchangeMic, exchangeAcronym);

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Any());

            var security = securities.First();

            Assert.AreEqual(exchangeMic, security.ExchangeMic);
            Assert.AreEqual(exchangeAcronym, security.ExchangeAcronym);
        }

        [TestMethod]
        public async Task ValidateTickersWhiteSpacesGetSecuritiesByExchangeTestAsync()
        {
            const string EXCHANGE_MIC = "XNYS";
            const string EXCHANGE_ACRONYM = "NYSE";

            var securities = await _client.GetSecuritiesByExchangeAsync(EXCHANGE_MIC, EXCHANGE_ACRONYM);

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Any());

            var tickersWithLeadingTrailingWhiteSpaces = securities.Where(x => x.Symbol.StartsWith(" ") || x.Symbol.EndsWith(" ")).Count();

            Assert.AreEqual(tickersWithLeadingTrailingWhiteSpaces, 0);
        }
    }
}
