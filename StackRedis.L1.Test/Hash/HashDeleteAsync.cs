using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashDeleteAsync : UnitTestBase
    {
        [TestMethod]
        public async Task HashDeleteAsync_Simple()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);

            //value1 should be mem cached
            _memDb.HashDelete("hashKey", "key1");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.IsFalse((await _memDb.HashGetAsync("hashKey", "key1")).HasValue);
            Assert.AreEqual(3, CallsByMemDb); //Has to go to redis for it

            //Ensure the original second value is untouched
            Assert.AreEqual("value2", (string)await _memDb.HashGetAsync("hashKey", "key2"));
            Assert.AreEqual(4, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashDelete_ByOtherClient()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2")});
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);

            //value1 should be mem cached
            await _otherClientDb.HashDeleteAsync("hashKey", "key1");

            await Task.Delay(50);
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse((await _memDb.HashGetAsync("hashKey", "key1")).HasValue);
            Assert.AreEqual(2, CallsByMemDb); //Has to go to redis for it

            //Ensure the original second value is untouched
            Assert.AreEqual("value2", (string)await _memDb.HashGetAsync("hashKey", "key2"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashDelete_InMemKeyLeftAlone()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1", "key2" });
            Assert.AreEqual(1, CallsByMemDb);

            //Keys should both be in memory
            Assert.IsTrue((await _memDb.HashGetAsync("hashKey", "key1")).HasValue);
            Assert.IsTrue((await _memDb.HashGetAsync("hashKey", "key2")).HasValue);
            Assert.AreEqual(1, CallsByMemDb);

            //Delete key1
            await _memDb.HashDeleteAsync("hashKey", "key1");
            Assert.AreEqual(2, CallsByMemDb);

            //Key2 should still be in cache
            Assert.AreEqual("value2", (string)await _memDb.HashGetAsync("hashKey", "key2"));
            Assert.AreEqual(2, CallsByMemDb);
        }
    }
}
