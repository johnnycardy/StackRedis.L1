using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringAppend: UnitTestBase
    {
        [TestMethod]
        public void StringAppend_Simple()
        {
            _memDb.PauseKeyspaceNotifications();

            _memDb.StringSet("key", "a");
            Assert.AreEqual(1, _redisDb.Calls);

            _memDb.StringAppend("key", "b");
            Assert.AreEqual(2, _redisDb.Calls);

            //It should not go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(2, _redisDb.Calls);
            Assert.AreEqual(result, "ab");
        }
        
        [TestMethod]
        public async Task StringAppend_InRedis_Notification()
        {
            _memDb.StringSet("key", "a");
            Assert.AreEqual(1, _redisDb.Calls);

            _redisDb.StringAppend("key", "b");
            Assert.AreEqual(2, _redisDb.Calls);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(3, _redisDb.Calls);
            Assert.AreEqual(result, "ab");
        }

        [TestMethod]
        public void StringAppend_DoesNotExist()
        {
            _memDb.PauseKeyspaceNotifications();
            
            _memDb.StringAppend("key", "a");
            Assert.AreEqual(1, _redisDb.Calls);

            //It should not go back to redis to re-request the string
            string result = _memDb.StringGet("key");
            Assert.AreEqual(1, _redisDb.Calls);
            Assert.AreEqual(result, "a");
        }

        [TestMethod]
        public async Task StringAppendAsync_Simple()
        {
            _memDb.PauseKeyspaceNotifications();

            await _memDb.StringSetAsync("key", "a");
            Assert.AreEqual(1, _redisDb.Calls);

            await _memDb.StringAppendAsync("key", "b");
            Assert.AreEqual(2, _redisDb.Calls);

            //It should not go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(2, _redisDb.Calls);
            Assert.AreEqual(result, "ab");
        }

        [TestMethod]
        public async Task StringAppendAsync_InRedis_Notification()
        {
            await _memDb.StringSetAsync("key", "a");
            Assert.AreEqual(1, _redisDb.Calls);

            await _redisDb.StringAppendAsync("key", "b");
            Assert.AreEqual(2, _redisDb.Calls);

            //Give it a moment to propagate
            await Task.Delay(50);

            //It should go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(3, _redisDb.Calls);
            Assert.AreEqual(result, "ab");
        }

        [TestMethod]
        public async Task StringAppendAsync_DoesNotExist()
        {
            _memDb.PauseKeyspaceNotifications();

            await _memDb.StringAppendAsync("key", "a");
            Assert.AreEqual(1, _redisDb.Calls);

            //It should not go back to redis to re-request the string
            string result = await _memDb.StringGetAsync("key");
            Assert.AreEqual(1, _redisDb.Calls);
            Assert.AreEqual(result, "a");
        }
    }
}
