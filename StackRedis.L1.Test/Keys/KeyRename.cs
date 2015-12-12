using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyRename : UnitTestBase
    {
        [TestMethod]
        public void KeyRename_Simple()
        {
            //Create a key
            _memDb.StringSet("blah_key1", "str");
            Assert.AreEqual(1, _redisDb.Calls);

            //Rename it
            _memDb.KeyRename("blah_key1", "blah_key2");
            Assert.AreEqual(2, _redisDb.Calls, "One extra call required to rename key");

            //Check it's renamed in memory - should not need to go to redis
            Assert.AreEqual((string)_memDb.StringGet("blah_key2"), "str");
            Assert.AreEqual(2, _redisDb.Calls, "No extra calls should be required to get the renamed key");

            //Check it's renamed in redis
            Assert.IsNull((string)_redisDb.StringGet("blah_key1"));
        }


        [TestMethod]
        public async Task KeyRename_ExpiryStillSet()
        {
            _memDb.StringSet("key1", "value1");
            
            //Now expire it
            _memDb.KeyExpire("key1", DateTime.UtcNow.AddMilliseconds(20));
            Assert.AreEqual(2, _redisDb.Calls);

            //Rename the key
            _memDb.KeyRename("key1", "key2");
            Assert.IsFalse(_memDb.KeyExists("key1"));

            //Wait the time for expiry
            await Task.Delay(25);

            //It should have now expired
            Assert.IsFalse(_memDb.StringGet("key2").HasValue);
        }

        [TestMethod]
        public void KeyRename_NewKeyAlreadyExists()
        {
            _memDb.StringSet("key1", "value1");
            _memDb.StringSet("key2", "value2");
            
            //Rename the key
            _memDb.KeyRename("key1", "key2");
            Assert.IsFalse(_memDb.KeyExists("key1"));
            Assert.IsTrue(_memDb.KeyExists("key2"));
            
            //key2 should have key1's value
            Assert.AreEqual("value1", (string)_memDb.StringGet("key2"));
        }
    }
}
