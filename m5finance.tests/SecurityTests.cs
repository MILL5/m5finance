using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace M5Finance.Tests
{
    [TestClass]
    public class SecurityTests
    {
        private readonly SecurityService _service;

        public SecurityTests()
        {
            _service = new SecurityService();
        }

        [TestMethod]
        public async Task GetSecuritiesTestAsync()
        {
            var securities = await _service.GetSecuritiesAsync();

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Count() > 0);
        }

        [TestMethod]
        public async Task GetTickersTestAsync()
        {
            var tickers = await _service.GetTickersAsync();
            var tickersDistinct = tickers.Distinct();

            Assert.IsNotNull(tickers);
            Assert.IsTrue(tickers.Count() > 0);
            Assert.AreEqual(tickers.Count(), tickersDistinct.Count());
        }

        [TestMethod]
        public async Task GetExchangeMicCodesTestAsync()
        {
            var exchanges = await _service.GetExchangeMicCodesAsync();

            Assert.IsNotNull(exchanges);
            Assert.IsTrue(exchanges.Count() > 0);
        }

    }
}
