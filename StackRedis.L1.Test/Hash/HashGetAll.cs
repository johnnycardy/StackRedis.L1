using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashGetAll : UnitTestBase
    {
        [TestMethod]
        public void HashGetAll_Simple()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            var all = _memDb.HashGetAll("hashKey");
            Assert.AreEqual("key1", (string)all[0].Name);
            Assert.AreEqual("value1", (string)all[0].Value);
            Assert.AreEqual("key2", (string)all[1].Name);
            Assert.AreEqual("value2", (string)all[1].Value);
            Assert.AreEqual(1, CallsByMemDb);
            
            //Retrieve an individual value
            Assert.AreEqual("value1", (string)_memDb.HashGet("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void HashGetAll_ClearsOldItems()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            var all = _memDb.HashGetAll("hashKey");
            Assert.AreEqual(1, CallsByMemDb);

            //Delete an item in redis
            _redisDirectDb.HashDelete("hashKey", "key1");
            all = _memDb.HashGetAll("hashKey");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(1, all.Length);
            Assert.AreEqual("key2", (string)all[0].Name);
            Assert.AreEqual("value2", (string)all[0].Value);
        }
    }
}
