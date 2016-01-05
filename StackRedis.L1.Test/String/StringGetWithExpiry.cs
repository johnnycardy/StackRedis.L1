using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StackRedis.L1.Test.String
{
    [TestClass]
    public class StringGetWithExpiry : UnitTestBase
    {
        [TestMethod]
        public void StringGetWithExpiry_StringAndExpirySetViaRedis()
        {
            _redisDirectDb.StringSet("key1", "val", TimeSpan.FromDays(1));
            Assert.AreEqual(0, CallsByMemDb);

            var result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsTrue(result.Expiry.HasValue);
            Assert.AreEqual(1, (int)Math.Round(result.Expiry.Value.TotalDays));
            Assert.AreEqual(1, CallsByMemDb);

            //Request it again from memory
            result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsTrue(result.Expiry.HasValue);
            Assert.AreEqual(1, (int)Math.Round(result.Expiry.Value.TotalDays));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void StringGetWithExpiry_StringAndNoExpirySetViaRedis()
        {
            _redisDirectDb.StringSet("key1", "val");
            Assert.AreEqual(0, CallsByMemDb);

            var result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsFalse(result.Expiry.HasValue);
            Assert.AreEqual(1, CallsByMemDb);

            //Request it again from memory
            result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsFalse(result.Expiry.HasValue);
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void StringGetWithExpiry_StringAndExpirySetInMem()
        {
            _memDb.StringSet("key1", "val", TimeSpan.FromDays(1));
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsTrue(result.Expiry.HasValue);
            Assert.AreEqual(1, (int)Math.Round(result.Expiry.Value.TotalDays), "Expiry should be 1 day");
            Assert.AreEqual(1, CallsByMemDb, "Should not have made another call to redis");
        }

        [TestMethod]
        public void StringGetWithExpiry_StringAndNoExpirySetInMem()
        {
            _memDb.StringSet("key1", "val");
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual("val", (string)result.Value);
            Assert.IsFalse(result.Expiry.HasValue);
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void StringGetWithExpiry_NoStringSet()
        {
            var result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(result.Value.HasValue);
            Assert.IsFalse(result.Expiry.HasValue);

            //Call it again - missing keys are not cached
            result = _memDb.StringGetWithExpiry("key1");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(result.Value.HasValue);
            Assert.IsFalse(result.Expiry.HasValue);
        }
    }
}
