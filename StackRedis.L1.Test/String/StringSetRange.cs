using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringSetRange : UnitTestBase
    {
        [TestMethod]
        public void StringSetRange_Simple()
        {
            _memDb.StringSet("key", "value", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.StringSetRange("key", 2, "x");
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreNotEqual(result, "value");
        }


        [TestMethod]
        public async Task StringSetRange_InRedis_Notification()
        {
            _memDb.StringSet("key", "value", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.StringSetRange("key", 2, "x");
            Assert.AreEqual(1, CallsByMemDb);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreNotEqual(result, "value");
        }

        [TestMethod]
        public async Task StringSetRangeAsync_Simple()
        {
            await _memDb.StringSetAsync("key", "value", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.StringSetRangeAsync("key", 2, "x");
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreNotEqual(result, "value");
        }
    }
}
