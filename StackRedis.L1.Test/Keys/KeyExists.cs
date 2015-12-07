using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyExists : UnitTestBase
    {
        [TestMethod]
        public void KeyExists_False()
        {
            Assert.IsFalse(_memDb.KeyExists("key1"));

            //It will have gone to redis to check
            Assert.AreEqual(1, _redisDb.Calls);

            //Try again - it will have go to redis to check again
            Assert.IsFalse(_memDb.KeyExists("key1"));
            Assert.AreEqual(2, _redisDb.Calls);
        }

        [TestMethod]
        public void KeyExists_True_StringAdd()
        {
            Assert.IsFalse(_memDb.KeyExists("key1"));
            Assert.AreEqual(1, _redisDb.Calls);

            _memDb.StringSet("key1", "value1");
            Assert.AreEqual(2, _redisDb.Calls);

            //We should be able to tell that it exists without going to redis
            Assert.IsTrue(_memDb.KeyExists("key1"));
            Assert.AreEqual(2, _redisDb.Calls);
        }

        [TestMethod]
        public void KeyExists_False_KeyDelete()
        {
            _memDb.StringSet("key1", "value1");
            Assert.AreEqual(1, _redisDb.Calls);

            Assert.IsTrue(_memDb.KeyExists("key1"));
            Assert.AreEqual(1, _redisDb.Calls);

            //Now delete it
            _memDb.KeyDelete("key1");

            //It will have to go back to redis to check that the key is deleted
            Assert.IsFalse(_memDb.KeyExists("key1"));
        }
    }
}
