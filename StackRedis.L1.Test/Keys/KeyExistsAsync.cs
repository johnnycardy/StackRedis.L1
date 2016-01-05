using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyExistsAsync : UnitTestBase
    {
        [TestMethod]
        public async Task KeyExistsAsync_False()
        {
            Assert.IsFalse(await _memDb.KeyExistsAsync("key1"));

            //It will have gone to redis to check
            Assert.AreEqual(1, CallsByMemDb);

            //Try again - it will have go to redis to check again
            Assert.IsFalse(await _memDb.KeyExistsAsync("key1"));
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task KeyExistsAsync_True_StringAdd()
        {
            Assert.IsFalse(await _memDb.KeyExistsAsync("key1"));
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.StringSetAsync("key1", "value1");
            Assert.AreEqual(2, CallsByMemDb);

            //We should be able to tell that it exists without going to redis
            Assert.IsTrue(await _memDb.KeyExistsAsync("key1"));
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task KeyExistsAsync_False_KeyDelete()
        {
            _memDb.StringSet("key1", "value1");
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(await _memDb.KeyExistsAsync("key1"));
            Assert.AreEqual(1, CallsByMemDb);

            //Now delete it
            await _memDb.KeyDeleteAsync("key1");

            //It will have to go back to redis to check that the key is deleted
            Assert.IsFalse(await _memDb.KeyExistsAsync("key1"));
        }
    }
}
