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
        private const string EX_MIC_XNYS = "XNYS";

        private const string EX_ACRO_XNYS = "NYSE";

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

        [TestMethod]
        public async Task GetSecuritiesByExchangeTestAsync()
        {
            var securities = await _client.GetSecuritiesByExchangeAsync(EX_MIC_XNYS, EX_ACRO_XNYS);

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Any());

            var security = securities.First();

            Assert.AreEqual(EX_MIC_XNYS, security.ExchangeMic);
            Assert.AreEqual(EX_ACRO_XNYS, security.ExchangeAcronym);
        }
    }
}
