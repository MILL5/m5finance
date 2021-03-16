using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;

namespace M5Finance.Tests
{
    [TestClass]
    public class EdgarTests
    {
        private readonly EdgarClient _client;

        public EdgarTests()
        {
            _client = new EdgarClient();
        }

        [TestMethod]
        public async Task GetEdgarFilingsTestAsync()
        {
            var lookup = await _client.GetEdgarFilingsAsync();
            lookup.ShouldNotBeNull();

            var filings = lookup.GetFilingsByForm("SC 13G");
            filings.ShouldNotBeNull();
        }
    }
}
