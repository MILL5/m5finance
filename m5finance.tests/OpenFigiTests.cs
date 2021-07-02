using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static M5Finance.Tests.TestManager;

namespace M5Finance.Tests
{
    [TestClass]
    public class OpenFigiTests
    {
        private const string EXCHANGE_CODE_US = "US";

        private readonly IOpenFigiClient _client;
       
        public OpenFigiTests()
        {
            _client = GetService<IOpenFigiClient>();
        }

        [TestMethod]
        public async Task GetOpenFigiSecuritiesTestAsync()
        {
            var requestList = new List<OpenFigiRequest>()
            {
                new OpenFigiRequest{ IdType = "ID_BB_GLOBAL", IdValue = "BBG004P64PB8" },
                new OpenFigiRequest{ IdType = "ID_BB_GLOBAL", IdValue = "BBG000DD3805" },
            };

            var figiInstrumentList = await _client.GetFigiMappingsAsync(requestList);

            Assert.IsNotNull(figiInstrumentList);
            Assert.IsTrue(figiInstrumentList.Count() == 2);
        }

        [TestMethod]
        public async Task GetMultipleOpenFigiSecuritiesTestAsync()
        {
            var requestList = new List<OpenFigiRequest>()
            {
                new OpenFigiRequest{ IdType = "ID_BB_GLOBAL", IdValue = "BBG004P64PB8" },
                new OpenFigiRequest{ IdType = "ID_BB_GLOBAL", IdValue = "BBG000DD3805" },
            };

            IEnumerable<OpenFigiInstrument> figiInstrumentList = null;

            for (int i = 0; i < 10; i++)
            {
                figiInstrumentList = await _client.GetFigiMappingsAsync(requestList);

                _client.CurrentLimits.ApiLimiter.TotalCount.ShouldBeGreaterThan(0);
                _client.CurrentLimits.ApiLimiter.CallsPerMinute.ShouldBeGreaterThan(0);
            }

            _ = _client.CurrentLimits.ApiLimiter.CallsPerMinute;

            Assert.IsNotNull(figiInstrumentList);
            Assert.IsTrue(figiInstrumentList.Count() == 2);
        }

        [TestMethod]
        public async Task GetOpenFigiSecuritiesTestAsyncThrowNullErrorAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await _client.GetFigiMappingsAsync(null);
            });
        }

        [TestMethod]
        public async Task GetOpenFigiSecuritiesTestAsyncThrowRangeErrorAsync()
        {
            var requestList = new List<OpenFigiRequest>();

            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _client.GetFigiMappingsAsync(requestList);
            });
        }

        [TestMethod]
        public async Task GetFigiMappingsForExchangeSucceedsAsync()
        {
            var result = await _client.GetFigiMappingsForExchangeAsync(EXCHANGE_CODE_US);

            Assert.IsInstanceOfType(result, typeof(OpenFigiResponseV3));
            Assert.IsInstanceOfType(result.Data.First(), typeof(OpenFigiInstrumentV3));
            Assert.AreEqual(100, result.Data.Count);
        }

        [TestMethod]
        public async Task GetFigiMappingsForExchangePaginationSucceedsAsync()
        {
            var result1 = await _client.GetFigiMappingsForExchangeAsync(EXCHANGE_CODE_US);

            Assert.IsInstanceOfType(result1, typeof(OpenFigiResponseV3));
            Assert.IsInstanceOfType(result1.Data.First(), typeof(OpenFigiInstrumentV3));
            Assert.AreEqual(100, result1.Data.Count);

            var result2 = await _client.GetFigiMappingsForExchangeAsync(EXCHANGE_CODE_US, result1.Next);

            Assert.IsInstanceOfType(result2, typeof(OpenFigiResponseV3));
            Assert.IsInstanceOfType(result2.Data.First(), typeof(OpenFigiInstrumentV3));
            Assert.AreEqual(100, result2.Data.Count);
        }

        [TestMethod]
        public async Task GetFigiMappingsForBadExchangeAsync()
        {
            var result = await _client.GetFigiMappingsForExchangeAsync("foo");

            Assert.IsInstanceOfType(result, typeof(OpenFigiResponseV3));
            Assert.IsFalse(result.Data.Any());
        }

        [TestMethod]
        public async Task GetFigiMappingsForExchangeNoExchangeAsync()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await _client.GetFigiMappingsForExchangeAsync(string.Empty);
            });
        }
    }
}
