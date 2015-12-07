using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using RedisL1.MemoryCache;
using System.Threading.Tasks;

namespace RedisL1.Test
{
    [TestClass]
    public class StringGetAsync : UnitTestBase
    {
        [TestMethod]
        public async Task StringGetAsync_Simple()
        {
            _redisDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));
            Assert.AreEqual(2, _redisDb.Calls);

            //value1 should be mem cached
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));
            Assert.AreEqual(2, _redisDb.Calls); //no extra call is made to redis
        }

        [TestMethod]
        public async Task StringGetAsync_Simple_Multi_BothValuesCached()
        {
            _redisDb.StringSet("key1", "value1");
            _redisDb.StringSet("key2", "value2");
            var values = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(3, _redisDb.Calls);

            //Remove keys
            _redisDb.KeyDelete(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual(4, _redisDb.Calls);

            //Original values should be cached without further calls to redis
            values = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(4, _redisDb.Calls);
        }

        [TestMethod]
        public async Task StringGetAsync_Simple_Multi_OneValueCached()
        {
            await _redisDb.StringSetAsync("key1", "value1");
            await _redisDb.StringSetAsync("key2", "value2");
            var values = await _memDb.StringGetAsync(new RedisKey[] { "key1" });
            Assert.AreEqual("value1", (string)values[0]);

            //only key 2 should need to be retrieved this time. We prove by removing key1
            _redisDb.KeyDelete("key1");
            await _redisDb.StringSetAsync("key2", "value2");

            //key1 should be cached, key2 should be retrieved
            values = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
        }
    }
}
