﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace StackRedis.L1.Test.String
{
    [TestClass]
    public class StringGetWithExpiryAsync : UnitTestBase
    {
        [TestMethod]
        public async Task StringGetWithExpiryAsync_StringAndExpirySetViaRedis()
        {
            await _redisDirectDb.StringSetAsync("key1", "val", TimeSpan.FromDays(1));
            Assert.AreEqual(0, CallsByMemDb);

            var result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsTrue(result.Expiry.HasValue);
            Assert.AreEqual(1, (int)Math.Round(result.Expiry.Value.TotalDays));
            Assert.AreEqual(1, CallsByMemDb);

            //Request it again from memory
            result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsTrue(result.Expiry.HasValue);
            Assert.AreEqual(1, (int)Math.Round(result.Expiry.Value.TotalDays));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task StringGetWithExpiryAsync_StringAndNoExpirySetViaRedis()
        {
            await _redisDirectDb.StringSetAsync("key1", "val");
            Assert.AreEqual(0, CallsByMemDb);

            var result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsFalse(result.Expiry.HasValue);
            Assert.AreEqual(1, CallsByMemDb);

            //Request it again from memory
            result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsFalse(result.Expiry.HasValue);
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task StringGetWithExpiryAsync_StringAndExpirySetInMem()
        {
            await _memDb.StringSetAsync("key1", "val", TimeSpan.FromDays(1), When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsTrue(result.Expiry.HasValue);
            Assert.AreEqual(1, (int)Math.Round(result.Expiry.Value.TotalDays));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task StringGetWithExpiryAsync_StringAndNoExpirySetInMem()
        {
            await _memDb.StringSetAsync("key1", "val", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsFalse(result.Expiry.HasValue);
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task StringGetWithExpiryAsync_NoStringSet()
        {
            var result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(result.Value.HasValue);
            Assert.IsFalse(result.Expiry.HasValue);

            //Call it again
            result = await _memDb.StringGetWithExpiryAsync("key1");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(result.Value.HasValue);
            Assert.IsFalse(result.Expiry.HasValue);
        }
    }
}
