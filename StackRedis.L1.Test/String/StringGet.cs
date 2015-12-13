using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringGet : UnitTestBase
    {
        [TestMethod]
        public void StringGet_Simple()
        {
            _redisDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));
            Assert.AreEqual(2, _redisDb.Calls);

            //value1 should be mem cached
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));
            Assert.AreEqual(2, _redisDb.Calls); //no extra call is made to redis
        }

        [TestMethod]
        public async Task StringGet_StringChangedInRedis()
        {
            //Set it and retrieve it into memory
            _redisDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));

            //Now change it in redis
            _redisDb.StringSet("key1", "value2");

            //Wait for it to propagate and re-retrieve
            await Task.Delay(50);
            Assert.AreEqual("value2", (string)_memDb.StringGet("key1"));
        }

        [TestMethod]
        public void StringGet_Simple_Multi_BothValuesCached()
        {
            _redisDb.StringSet("key1", "value1");
            _redisDb.StringSet("key2", "value2");
            var values = _memDb.StringGet(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(3, _redisDb.Calls);

            //Remove keys
            _redisDb.KeyDelete(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual(4, _redisDb.Calls);

            //Original values should be cached without further calls to redis
            values = _memDb.StringGet(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
            Assert.AreEqual(4, _redisDb.Calls);
        }

        [TestMethod]
        public void StringGet_Simple_Multi_OneValueCached()
        {
            _redisDb.StringSet("key1", "value1");
            _redisDb.StringSet("key2", "value2");
            var values = _memDb.StringGet(new RedisKey[] { "key1" });
            Assert.AreEqual("value1", (string)values[0]);

            //only key 2 should need to be retrieved this time. We prove by removing key1 from redis only - not memory
            _memDb.DBData.Listener.Paused = true;
            _redisDb.KeyDelete("key1");
            _redisDb.StringSet("key2", "value2");

            //key1 should be cached, key2 should be retrieved
            values = _memDb.StringGet(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
        }

        [TestMethod]
        public async Task StringGet_WithExpiry()
        {
            //Set in redis with an expiry
            _redisDb.StringSet("key", "value1", TimeSpan.FromMilliseconds(30));

            //Pull into memory
            Assert.AreEqual("value1", (string)_memDb.StringGet("key"));
            Assert.AreEqual(2, _redisDb.Calls);

            //Test that it's set in mem
            Assert.AreEqual("value1", (string)_memDb.StringGet("key"));
            Assert.AreEqual(2, _redisDb.Calls);

            await Task.Delay(100);

            //Get it again - should go back to redis, where it's now not set since it's expired
            Assert.IsFalse(_memDb.StringGet("key").HasValue);
            Assert.AreEqual(3, _redisDb.Calls);
        }
    }
}
