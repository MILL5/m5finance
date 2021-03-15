using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M5Finance.Tests
{
    [TestClass]
    public class OpenFigiTests
    {
        private readonly OpenFigiClient _client;

        public OpenFigiTests()
        {
            _client = new OpenFigiClient();
        }

        [TestMethod]
        public async Task GetOpenFigiSecuritiesTestAsync()
        {
            var list = new List<OpenFigiRequest>()
                {
                   new OpenFigiRequest("TICKER", "MSFT"),
                };

            var figiInstrumentList = await _client.GetFigiMappingsAsync(list);

            Assert.IsNotNull(figiInstrumentList);
            Assert.IsTrue(figiInstrumentList.Count() == 40);
        }
    }
}
