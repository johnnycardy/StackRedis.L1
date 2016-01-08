using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetContains : UnitTestBase
    {
        [TestMethod]
        public void SetContains_ValueInRedis()
        {
            _redisDirectDb.SetAdd("set1", "value1");
            Assert.IsTrue(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void SetContains_ValueInMemory()
        {
            _memDb.SetAdd("set1", "value1");
            Assert.IsTrue(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);
        }
    }
}