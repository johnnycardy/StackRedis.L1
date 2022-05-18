﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyDelete : UnitTestBase
    {
        [TestMethod]
        public void KeyDelete_AfterSet()
        {
            _memDb.StringSet("key1", "value1", null, When.Always);
            
            //value1 should be mem cached
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));

            //Delete a different key
            _memDb.KeyDelete("key2");

            //Ensure it's still cached
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));

            //Delete it
            _memDb.KeyDelete("key1");

            //It should be gone
            RedisValue val = _memDb.StringGet("key1");
            Assert.IsFalse(val.HasValue);
        }

        [TestMethod]
        public async Task KeyDeleteAsync_AfterSet()
        {
            await _memDb.StringSetAsync("key1", "value1", null, When.Always);

            //value1 should be mem cached
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));

            //Delete a different key
            await _memDb.KeyDeleteAsync("key2");

            //Ensure it's still cached
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));

            //Delete it
            await _memDb.KeyDeleteAsync("key1");

            //It should be gone
            RedisValue val = await _memDb.StringGetAsync("key1");
            Assert.IsFalse(val.HasValue);
        }

    }
}
