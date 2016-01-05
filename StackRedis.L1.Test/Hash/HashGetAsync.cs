using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashGetAsync : UnitTestBase
    {
        [TestMethod]
        public async Task HashGetAsync_Simple()
        {
            await _redisDirectDb.HashSetAsync("hashKey", "key1", "value1");
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);

            //value1 should be mem cached
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb); //no extra call is made to redis
        }

        [TestMethod]
        public async Task HashGetAsync_StringChangedInRedis()
        {
            //Set it and retrieve it into memory
            _redisDirectDb.HashSet("hashKey", "key1", "value1");
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
            
            //Now change it via the other client
            _otherClientDb.HashSet("hashKey", "key1", "value2");

            //Wait for it to propagate and re-retrieve
            await Task.Delay(50);
            Assert.AreEqual("value2", (string)await _memDb.HashGetAsync("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashGetAsync_Simple_Multi_BothValuesCached()
        {
            await _redisDirectDb.HashSetAsync("hashKey", "key1", "value1");
            await _redisDirectDb.HashSetAsync("hashKey", "key2", "value2");
            var values = await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(1, CallsByMemDb);
            
            //Original values should be cached without further calls to redis
            values = await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashGetAsync_Multi()
        {
            //Use the memory DB to set keys
            await _memDb.HashSetAsync("hashKey", "key1", "value1");
            await _memDb.HashSetAsync("hashKey", "key2", "value2");

            //Wait for notifications to arrive from setting both keys
            await Task.Delay(50);

            Assert.AreEqual(2, CallsByMemDb);
            var values = await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashGetAsync_Multi_OtherClient()
        {
            //Use the memory DB to set keys
            await _memDb.HashSetAsync("hashKey", "key1", "value1");
            await _otherClientDb.HashSetAsync("hashKey", "key2", "value2");

            //Wait for notifications to arrive from setting both keys
            await Task.Delay(50);

            Assert.AreEqual(1, CallsByMemDb);
            var values = await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashGetAsync_Simple_Multi_OneValueCached()
        {
            await _redisDirectDb.HashSetAsync("hashKey", "key1", "value1");
            await _redisDirectDb.HashSetAsync("hashKey", "key2", "value2");
            var values = await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1" });
            Assert.AreEqual("value1", (string)values[0]);

            //only key 2 should need to be retrieved this time. We prove by removing key1 from redis only - not memory
            await _redisDirectDb.HashDeleteAsync("hashKey", "key1");
            await _redisDirectDb.HashSetAsync("hashKey", "key2", "value2");

            //key1 should be cached, key2 should be retrieved
            values = await _memDb.HashGetAsync("hashKey", new RedisValue[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
        }

        [TestMethod]
        public async Task HashGetAsync_WithExpiry()
        {
            //Set in redis with an expiry
            await _redisDirectDb.HashSetAsync("hashKey", "key_exp", "value1");
            await _redisDirectDb.KeyExpireAsync("hashKey", TimeSpan.FromMilliseconds(30));

            //Pull into memory
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key_exp"));
            Assert.AreEqual(1, CallsByMemDb, "value1 should be pulled into memory");

            //Test that it's set in mem
            Assert.AreEqual("value1", (string)await _memDb.HashGetAsync("hashKey", "key_exp"));
            Assert.AreEqual(1, CallsByMemDb, "value1 should be already set in memory");

            await Task.Delay(200);

            //Get it again - should go back to redis, where it's now not set since it's expired
            Assert.IsFalse((await _memDb.HashGetAsync("hashKey", "key_exp")).HasValue);
            Assert.AreEqual(2, CallsByMemDb);
        }
    }
}
