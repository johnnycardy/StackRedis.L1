using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringGetAsync : UnitTestBase
    {
        [TestMethod]
        public async Task StringGetAsync_Simple()
        {
            _redisDirectDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));
            Assert.AreEqual(1, CallsByMemDb);

            //value1 should be mem cached
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));
            Assert.AreEqual(1, CallsByMemDb); //no extra call is made to redis
        }

        [TestMethod]
        public async Task StringGetAsync_Simple_Multi_BothValuesCached()
        {
            _redisDirectDb.StringSet("key1", "value1");
            _redisDirectDb.StringSet("key2", "value2");
            var values = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(1, CallsByMemDb);
            
            //Original values should be cached without further calls to redis
            values = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task StringGetAsync_Simple_Multi_OneValueCached()
        {
            await _redisDirectDb.StringSetAsync("key1", "value1");
            await _redisDirectDb.StringSetAsync("key2", "value2");
            var values = await _memDb.StringGetAsync(new RedisKey[] { "key1" });
            Assert.AreEqual("value1", (string)values[0]);

            //only key 2 should need to be retrieved this time. We prove by removing key1 from redis and not memory
            _redisDirectDb.KeyDelete("key1");
            await _redisDirectDb.StringSetAsync("key2", "value2");

            //key1 should be cached, key2 should be retrieved
            values = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
        }
    }
}
