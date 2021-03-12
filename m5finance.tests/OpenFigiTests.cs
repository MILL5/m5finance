using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M5Finance.Tests
{
    [TestClass]
    public class OpenFigiTests
    {
        private readonly OPENFIGIClient _client;

        public OpenFigiTests()
        {
            _client = new OPENFIGIClient();
        }

        [TestMethod]
        public async Task GetOpenFigiSecuritiesTestAsync()
        {
            var list = new List<OpenFIGIRequest>()
                {
                    new OpenFIGIRequest("TICKER", "MSFT"),
                };

            var figiInstrumentList = await _client.GetFigiMappingsAsync(new List<OpenFIGIRequest>());

            Assert.IsNotNull(figiInstrumentList);
            Assert.IsTrue(figiInstrumentList.Count() == 40);
        }
    }
}
