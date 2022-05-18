using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringIncrement: UnitTestBase
    {
        [TestMethod]
        public void StringIncrement_Double_Simple()
        {
            _memDb.StringSet("key", "1", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.StringIncrement("key", 1.5);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "2.5");
        }


        [TestMethod]
        public async Task StringIncrement_Double_InRedis_Notification()
        {
            _memDb.StringSet("key", "1", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.StringIncrement("key", 1.5);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(result, "2.5");
        }

        [TestMethod]
        public async Task StringIncrementAsync_Double_Simple()
        {
            await _memDb.StringSetAsync("key", "1", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.StringIncrementAsync("key", 1.5);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "2.5");
        }

        [TestMethod]
        public void StringIncrement_Long_Simple()
        {
            _memDb.StringSet("key", "1", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.StringIncrement("key", 2);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "3");
        }


        [TestMethod]
        public async Task StringIncrement_Long_InRedis_Notification()
        {
            _memDb.StringSet("key", "1", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.StringIncrement("key", 2);
            Assert.AreEqual(1, CallsByMemDb);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(result, "3");
        }

        [TestMethod]
        public async Task StringIncrementAsync_Long_Simple()
        {
            await _memDb.StringSetAsync("key", "1", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.StringIncrementAsync("key", 2);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "3");
        }
    }
}
