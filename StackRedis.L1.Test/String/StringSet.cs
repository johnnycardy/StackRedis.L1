﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringSet : UnitTestBase
    {
        [TestMethod]
        public void StringSet_ThenGet()
        {
            _memDb.StringSet("key1", "value1", null, When.Always);

            Assert.AreEqual(1, CallsByMemDb);

            //remove key1 in Redis
            _redisDirectDb.KeyDelete("key1");
            Assert.AreEqual(1, CallsByMemDb);

            //value1 should be mem cached
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }


        [TestMethod]
        public void StringSetMulti_ThenGet()
        {
            _memDb.StringSet(new[] 
            {
                new System.Collections.Generic.KeyValuePair<RedisKey, RedisValue>("key1", "value1"),
                new System.Collections.Generic.KeyValuePair<RedisKey, RedisValue>("key2", "value2"),
            });

            Assert.AreEqual(1, CallsByMemDb);

            //remove key1 in Redis
            _redisDirectDb.KeyDelete("key1");

            //key1 should be mem cached
            var result = _memDb.StringGet(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)result[0]);
            Assert.AreEqual("value2", (string)result[1]);
            Assert.AreEqual(1, CallsByMemDb);
        }


    }
}
