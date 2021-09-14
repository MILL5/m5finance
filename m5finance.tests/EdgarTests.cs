using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace M5Finance.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EdgarTests
    {
        private readonly EdgarClient _client;

        public EdgarTests()
        {
            _client = new EdgarClient();
        }

        [Ignore]
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
