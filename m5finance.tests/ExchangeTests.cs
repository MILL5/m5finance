using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace M5Finance.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExchangeTests
    {
        private readonly IExchangeService _service;

        public ExchangeTests()
        {
            _service = new ExchangeService();
        }

        [TestMethod]
        public async Task GetExchangesTestAsync()
        {
            var exchanges = await _service.GetExchangesAsync();

            Assert.IsNotNull(exchanges);
            Assert.IsTrue(exchanges.Count() > 0);
        }

        [TestMethod]
        public async Task GetExchangesByMicCodeTestAsync()
        {
            const string NASDAQ_MICCODE = "XNAS";

            var exchange = await _service.GetExchangeByMicCodeAsync(NASDAQ_MICCODE);

            Assert.IsNotNull(exchange);
            Assert.AreEqual(exchange.Mic, NASDAQ_MICCODE);
        }

        [TestMethod]
        public async Task GetExchangesByAcronymTestAsync()
        {
            const string NYSE_ACRONYM = "NYSE";

            var exchanges = await _service.GetExchangeByAcronymAsync(NYSE_ACRONYM);

            Assert.IsNotNull(exchanges);
            Assert.IsTrue(exchanges.Count() > 0);
        }

        [TestMethod]
        public async Task GetExchangeMicCodesTestAsync()
        {
            var miccodes = await _service.GetExchangeMicCodesAsync();

            Assert.IsNotNull(miccodes);
            Assert.IsTrue(miccodes.Count() > 0);
        }

        [TestMethod]
        public async Task GetExchangeAcronymsTestAsync()
        {
            var acronyms = await _service.GetExchangeAcronymsAsync();

            Assert.IsNotNull(acronyms);
            Assert.IsTrue(acronyms.Count() > 0);
        }
    }
}
