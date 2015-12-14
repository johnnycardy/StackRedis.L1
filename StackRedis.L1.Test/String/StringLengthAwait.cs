using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringLengthAsync : UnitTestBase
    {
        [TestMethod]
        public async Task StringLengthAsync_Simple()
        {
            await _memDb.StringSetAsync("key", "value");
            Assert.AreEqual(1, _redisDb.Calls);

            //length should be retrievable from cache
            Assert.AreEqual(5, (int)(await _memDb.StringLengthAsync("key")));
            Assert.AreEqual(1, _redisDb.Calls);
        }
        
        [TestMethod]
        public async Task StringLengthAsync_StringChangedInRedis()
        {
            await _memDb.StringSetAsync("key", "value");
            Assert.AreEqual(1, _redisDb.Calls);

            //Get the length from the cache
            Assert.AreEqual(5, (int)(await _memDb.StringLengthAsync("key")));
            Assert.AreEqual(1, _redisDb.Calls);

            await Task.Delay(1200);

            //Change the string in redis
            await _redisDb.StringSetAsync("key", "longer value");

            await Task.Delay(30); //Wait for the keyspace notification to remove the string

            //Length should be changed
            Assert.AreEqual(12, (int)(await _memDb.StringLengthAsync("key")));
        }
    }
}
