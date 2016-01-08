using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;
using System.Linq;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetScan : UnitTestBase
    {
        [TestMethod]
        public void SetScan_Simple()
        {
            //Add items in redis
            _redisDirectDb.SetAdd("set", new RedisValue[] { "value1", "value2"});
            var result = _memDb.SetScan("set").First();
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(string.IsNullOrEmpty(result)); //Either one should be returned

            Assert.IsTrue(_memDb.SetContains("set", "value1"));
            Assert.IsTrue(_memDb.SetContains("set", "value2"));
            Assert.AreEqual(2, CallsByMemDb); //Only go to redis once since First() was called earlier instead of eg. ToArray
        }


        [TestMethod]
        public void SetScan_WithPageSize()
        {
            //Add items in redis
            _redisDirectDb.SetAdd("set", new RedisValue[] { "value1", "value2" });
            var result = _memDb.SetScan("set", "*", 10, CommandFlags.None).First();
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(string.IsNullOrEmpty(result)); //Either one should be returned

            Assert.IsTrue(_memDb.SetContains("set", "value1"));
            Assert.IsTrue(_memDb.SetContains("set", "value2"));
            Assert.AreEqual(2, CallsByMemDb); //Only go to redis once since First() was called earlier instead of eg. ToArray
        }
    }
}