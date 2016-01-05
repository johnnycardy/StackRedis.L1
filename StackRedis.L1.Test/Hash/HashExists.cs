using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashExists : UnitTestBase
    {
        [TestMethod]
        public void HashExists_KeyPresent_NotInMemory()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(0, CallsByMemDb);
            
            Assert.IsTrue(_memDb.HashExists("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void HashExists_KeyPresent_InMemory()
        {
            _memDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(1, CallsByMemDb);

            //This is when HashExist is useful - the key is already in memory.
            Assert.IsTrue(_memDb.HashExists("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void HashExists_KeyMissing_NotInMemory()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(0, CallsByMemDb);
            
            Assert.IsFalse(_memDb.HashExists("hashKey", "key3"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void HashExists_KeyMissing_InMemory()
        {
            _memDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(1, CallsByMemDb);
            
            Assert.IsFalse(_memDb.HashExists("hashKey", "key3"));
            Assert.AreEqual(2, CallsByMemDb);
        }
    }
}
