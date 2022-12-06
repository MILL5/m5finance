using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace M5Finance.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NasdaqTraderTests
    {
        private readonly NTNasdaqClient _client;
        private readonly NTOtherClient _otherClient;

        public NasdaqTraderTests()
        {
            _client = new NTNasdaqClient();
            _otherClient = new NTOtherClient();
        }

        [TestMethod]
        [Ignore]
        public async Task GetNasdaqSecuritiesTestAsync()
        {
            var securities = await _client.GetSecuritiesAsync();

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Count() > 0);
        }

        [TestMethod]
        [Ignore]
        public async Task GetOtherSecuritiesTestAsync()
        {
            var securities = await _otherClient.GetSecuritiesAsync();

            Assert.IsNotNull(securities);
            Assert.IsTrue(securities.Count() > 0);
        }
    }
}
