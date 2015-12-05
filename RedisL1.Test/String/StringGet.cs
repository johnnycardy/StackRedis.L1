using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using RedisL1.MemoryCache;

namespace RedisL1.Test
{
    [TestClass]
    public class StringGet
    {
        private CallMonitoringRedisDatabase _redisDb;
        private RedisL1Database _memDb;

        [TestInitialize]
        public void SetUp()
        {
            _redisDb = UnitTestSetup.SetUp();
            _memDb = new RedisL1Database(_redisDb);
        }

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

            //only key 2 should need to be retrieved this time. We prove by removing key1
            _redisDb.KeyDelete("key1");
            _redisDb.StringSet("key2", "value2");

            //key1 should be cached, key2 should be retrieved
            values = _memDb.StringGet(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)values[0]);
            Assert.AreEqual("value2", (string)values[1]);
        }
    }
}
