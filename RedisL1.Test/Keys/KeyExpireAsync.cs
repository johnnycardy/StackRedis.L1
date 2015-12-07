﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using RedisL1.MemoryCache;
using System.Threading.Tasks;

namespace RedisL1.Test
{
    [TestClass]
    public class KeyExpireAsync : UnitTestBase
    {
        [TestMethod]
        public async Task KeyExpireAsync_DateTime()
        {
            await _memDb.StringSetAsync("key1", "value1");
            
            //value1 should be mem cached
            Assert.IsTrue((await _memDb.StringGetAsync("key1")).HasValue);

            //Now expire it
            await _memDb.KeyExpireAsync("key1", DateTime.UtcNow.AddMilliseconds(10));
            Assert.AreEqual(2, _redisDb.Calls);

            //Wait the 10 milliseconds for expiry
            await Task.Delay(15);

            Assert.IsFalse((await _memDb.StringGetAsync("key1")).HasValue);

            //Add it again and make sure it's back
            await _memDb.StringSetAsync("key1", "value1");
            Assert.IsTrue((await _memDb.StringGetAsync("key1")).HasValue);
        }

        [TestMethod]
        public async Task KeyExpireAsync_DateTime_Negative()
        {
            _memDb.StringSet("key1", "value1");

            //value1 should be mem cached
            Assert.IsTrue(_memDb.StringGet("key1").HasValue);

            //Now expire it
            await _memDb.KeyExpireAsync("key1", DateTime.UtcNow.AddDays(-10));
            
            Assert.IsFalse(_memDb.StringGet("key1").HasValue);
        }

        [TestMethod]
        public async Task KeyExpire_Timespan()
        {
            _memDb.StringSet("key1", "value1");
            
            //value1 should be mem cached
            Assert.IsTrue(_memDb.StringGet("key1").HasValue);

            //Now expire it
            await _memDb.KeyExpireAsync("key1", TimeSpan.FromMilliseconds(10));
            Assert.AreEqual(2, _redisDb.Calls);

            //Wait the 10 milliseconds for expiry
            await Task.Delay(15);

            Assert.IsFalse(_memDb.StringGet("key1").HasValue);

            //Add it again and make sure it's back
            _memDb.StringSet("key1", "value1");
            Assert.IsTrue(_memDb.StringGet("key1").HasValue);
        }

        [TestMethod]
        public async Task KeyExpire_Timespan_Negative()
        {
            _memDb.StringSet("key1", "value1");

            //value1 should be mem cached
            Assert.IsTrue(_memDb.StringGet("key1").HasValue);

            //Now expire it
            await _memDb.KeyExpireAsync("key1", TimeSpan.FromMilliseconds(-10));
            
            Assert.IsFalse(_memDb.StringGet("key1").HasValue);
        }

        [TestMethod]
        public async Task KeyExpire_ClearTimeout_Timespan()
        {
            //Set a key with a timeout
            _memDb.StringSet("key1", "value1", TimeSpan.FromMilliseconds(50));

            //Clear the timeout before it finishes
            await _memDb.KeyExpireAsync("key1", (TimeSpan?)null);

            //Wait the remaining timeout
            await Task.Delay(50);

            //Ensure it's still there
            Assert.IsTrue(_memDb.StringGet("key1").HasValue);
        }

        [TestMethod]
        public async Task KeyExpire_ClearTimeout_DateTime()
        {
            //Set a key with a timeout
            _memDb.StringSet("key1", "value1", TimeSpan.FromMilliseconds(50));

            //Clear the timeout before it finishes
            await _memDb.KeyExpireAsync("key1", (DateTime?)null);

            //Wait the remaining timeout
            await Task.Delay(50);

            //Ensure it's still there
            Assert.IsTrue(_memDb.StringGet("key1").HasValue);
        }
    }
}
