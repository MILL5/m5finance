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
    }
}
