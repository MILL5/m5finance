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
            Assert.IsTrue(securities.Count() > 0);
        }
    }
}
