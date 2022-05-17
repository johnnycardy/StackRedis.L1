using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringDecrement: UnitTestBase
    {
        [TestMethod]
        public void StringDecrement_Double_Simple()
        {
            _memDb.StringSet("key", "2.5", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.StringDecrement("key", 1.5);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "1");
        }


        [TestMethod]
        public async Task StringDecrement_Double_InRedis_Notification()
        {
            _memDb.StringSet("key", "2.5", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.StringDecrement("key", 1.5);
            Assert.AreEqual(1, CallsByMemDb);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(result, "1");
        }

        [TestMethod]
        public async Task StringDecrementAsync_Double_Simple()
        {
            await _memDb.StringSetAsync("key", "2.5", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.StringDecrementAsync("key", 1.5);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "1");
        }

        [TestMethod]
        public void StringDecrement_Long_Simple()
        {
            _memDb.StringSet("key", "3", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.StringDecrement("key", 2);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "1");
        }


        [TestMethod]
        public async Task StringDecrement_Long_InRedis_Notification()
        {
            _memDb.StringSet("key", "3", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.StringDecrement("key", 2);
            Assert.AreEqual(1, CallsByMemDb);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(result, "1");
        }

        [TestMethod]
        public async Task StringDecrementAsync_Long_Simple()
        {
            await _memDb.StringSetAsync("key", "3", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.StringDecrementAsync("key", 2);
            Assert.AreEqual(2, CallsByMemDb);

            //It should go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(result, "1");
        }
    }
}
