﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace M5Finance.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CikLookupTests
    {
        private readonly CIKMappingClient _client;

        public CikLookupTests()
        {
            _client = new CIKMappingClient();
        }

        [TestMethod]
        public async Task GetCikMappingFromTickerTestAsync()
        {
            const string TICKER = "MSFT";

            var lookup = await _client.GetCikMappingsAsync();

            Assert.IsNotNull(lookup);

            var cik = lookup.FromTickerToCik(TICKER);
            cik.ShouldNotBeNull();
            cik.Count().ShouldBeGreaterThan(0);

            var ticker = lookup.FromCikToTicker(cik[0]);
            ticker.ShouldNotBeNull();
            ticker[0].ShouldBeEquivalentTo(TICKER);
        }

        [TestMethod]
        public async Task GetCikMappingFromCikTestAsync()
        {
            const int CIK = 5513;

            var lookup = await _client.GetCikMappingsAsync();

            Assert.IsNotNull(lookup);

            var ticker = lookup.FromCikToTicker(CIK);
            ticker.ShouldNotBeNull();

            var cik = lookup.FromTickerToCik(ticker[0]);
            cik.ShouldNotBeNull();
            cik[0].ShouldBeEquivalentTo(CIK);
        }

        [TestMethod]
        public async Task FindOneToManyMappingsTestAsync()
        {
            var lookup = await _client.GetCikMappingsAsync();

            Assert.IsNotNull(lookup);

            var onetomany = lookup.FindOneToManyMappings();
            onetomany.ShouldNotBeNull();
            onetomany.Count().ShouldBeGreaterThan(0);
        }
    }
}
