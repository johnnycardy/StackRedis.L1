using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetPop : UnitTestBase
    {
        [TestMethod]
        public void SetPop_ValueInMem()
        {
            Assert.IsTrue(_memDb.SetAdd("set1", "value1"));
            Assert.AreEqual("value1", (string)_memDb.SetPop("set1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public void SetPop_ValueInRedis()
        {
            Assert.IsTrue(_redisDirectDb.SetAdd("set1", "value1"));
            Assert.AreEqual("value1", (string)_memDb.SetPop("set1"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetPop_ByOtherClient()
        {
            Assert.IsTrue(_memDb.SetAdd("set1", "value1"));
            _otherClientDb.SetPop("set1");
            Assert.AreEqual(1, CallsByMemDb);
            await Task.Delay(50);

            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetPopAsync_ValueInMem()
        {
            Assert.IsTrue(await _memDb.SetAddAsync("set1", "value1"));
            Assert.AreEqual("value1", (string)(await _memDb.SetPopAsync("set1")));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetPopAsync_ValueInRedis()
        {
            Assert.IsTrue(await _redisDirectDb.SetAddAsync("set1", "value1"));
            Assert.AreEqual("value1", (string)(await _memDb.SetPopAsync("set1")));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetPopAsync_ByOtherClient()
        {
            Assert.IsTrue(await _memDb.SetAddAsync("set1", "value1"));
            await _otherClientDb.SetPopAsync("set1");
            Assert.AreEqual(1, CallsByMemDb);
            await Task.Delay(50);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);
        }
    }
}