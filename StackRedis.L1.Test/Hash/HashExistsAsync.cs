using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashExistsAsync : UnitTestBase
    {
        [TestMethod]
        public async Task HashExistsAsync_KeyPresent_NotInMemory()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(0, CallsByMemDb);
            
            Assert.IsTrue(await _memDb.HashExistsAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashExistsAsync_KeyPresent_InMemory()
        {
            await _memDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(1, CallsByMemDb);

            //This is when HashExist is useful - the key is already in memory.
            Assert.IsTrue(await _memDb.HashExistsAsync("hashKey", "key1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashExistsAsync_KeyMissing_NotInMemory()
        {
            await _redisDirectDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(0, CallsByMemDb);
            
            Assert.IsFalse(await _memDb.HashExistsAsync("hashKey", "key3"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task HashExistsAsync_KeyMissing_InMemory()
        {
            await _memDb.HashSetAsync("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(1, CallsByMemDb);
            
            Assert.IsFalse(await _memDb.HashExistsAsync("hashKey", "key3"));
            Assert.AreEqual(2, CallsByMemDb);
        }
    }
}
