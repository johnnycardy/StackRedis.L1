using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyRenameAsync : UnitTestBase
    {
        [TestMethod]
        public async Task KeyRenameAsync_Simple()
        {
            //Create a key
            await _memDb.StringSetAsync("key1", "str");
            Assert.AreEqual(1, CallsByMemDb);

            //Rename it
            await _memDb.KeyRenameAsync("key1", "key2");
            Assert.AreEqual(2, CallsByMemDb);

            //Check it's renamed in memory - should not need to go to redis
            System.Diagnostics.Debug.WriteLine("About to get key2");
            Assert.AreEqual((string)(await _memDb.StringGetAsync("key2")), "str");
            Assert.AreEqual(2, CallsByMemDb, "No extra calls should be required to get the renamed key");

            //Check it's renamed in redis
            Assert.IsNull((string)await _redisDirectDb.StringGetAsync("key1"));
        }


        [TestMethod]
        public async Task KeyRenameAsync_ExpiryStillSet()
        {
            await _memDb.StringSetAsync("key1", "value1");

            //Now expire it
            await _memDb.KeyExpireAsync("key1", DateTime.UtcNow.AddMilliseconds(20));
            Assert.AreEqual(2, CallsByMemDb);

            //Rename the key
            await _memDb.KeyRenameAsync("key1", "key2");
            Assert.IsFalse(await _memDb.KeyExistsAsync("key1"));

            //Wait the time for expiry
            await Task.Delay(25);

            //It should have now expired
            Assert.IsFalse((await _memDb.StringGetAsync("key2")).HasValue);
        }

        [TestMethod]
        public async Task KeyRenameAsync_NewKeyAlreadyExists()
        {
            await _memDb.StringSetAsync("key1", "value1");
            await _memDb.StringSetAsync("key2", "value2");

            //Rename the key
            await _memDb.KeyRenameAsync("key1", "key2");
            Assert.IsFalse(await _memDb.KeyExistsAsync("key1"));
            Assert.IsTrue(await _memDb.KeyExistsAsync("key2"));

            //key2 should have key1's value
            Assert.AreEqual("value1", (string)await _memDb.StringGetAsync("key2"));
        }
    }
}
