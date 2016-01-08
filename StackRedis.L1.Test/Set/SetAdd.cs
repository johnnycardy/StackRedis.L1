using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetAdd : UnitTestBase
    {
        [TestMethod]
        public void SetAdd_Simple()
        {
            Assert.IsTrue(_memDb.SetAdd("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            //Calling it again should give false since it's already there
            Assert.IsFalse(_memDb.SetAdd("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            //Ensure it's present
            Assert.IsTrue(_redisDirectDb.SetContains("set1", "value1"));
        }

        [TestMethod]
        public void SetAdd_KeyAlreadyPresent()
        {
            _redisDirectDb.SetAdd("set1", "value1");
         
            //Calling it should give false since it's already there
            Assert.IsFalse(_memDb.SetAdd("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void SetAdd_OtherClient()
        {
            _memDb.SetAdd("set1", "value1");
            Assert.AreEqual(1, CallsByMemDb);
            _otherClientDb.SetAdd("set1", "value2");

            Assert.IsTrue(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(_memDb.SetContains("set1", "value2"));
            Assert.AreEqual(2, CallsByMemDb); //Has to go to redis to check
        }

        [TestMethod]
        public async Task SetAddAsync_Simple()
        {
            Assert.IsTrue(await _memDb.SetAddAsync("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            //Calling it again should give false since it's already there
            Assert.IsFalse(await _memDb.SetAddAsync("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            //Ensure it's present
            Assert.IsTrue(await _redisDirectDb.SetContainsAsync("set1", "value1"));
        }

        [TestMethod]
        public async Task SetAddAsync_KeyAlreadyPresent()
        {
            await _redisDirectDb.SetAddAsync("set1", "value1");

            //Calling it should give false since it's already there
            Assert.IsFalse(await _memDb.SetAddAsync("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);
        }
    }
}