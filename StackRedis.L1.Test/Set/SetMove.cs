using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetMove : UnitTestBase
    {
        [TestMethod]
        public void SetMove_InMem()
        {
            Assert.IsTrue(_memDb.SetAdd("set1", "value1"));
            Assert.IsTrue(_memDb.SetMove("set1", "set2", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(3, CallsByMemDb);

            Assert.IsTrue(_memDb.SetContains("set2", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public void SetMove_InRedis()
        {
            Assert.IsTrue(_redisDirectDb.SetAdd("set1", "value1"));
            Assert.IsTrue(_memDb.SetMove("set1", "set2", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsTrue(_memDb.SetContains("set2", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetMove_ByOtherClient()
        {
            Assert.IsTrue(_memDb.SetAdd("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(_otherClientDb.SetMove("set1", "set2", "value1"));
            await Task.Delay(50);
            
            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsTrue(_memDb.SetContains("set2", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }
        [TestMethod]
        public async Task SetMoveAsync_InMem()
        {
            Assert.IsTrue(await _memDb.SetAddAsync("set1", "value1"));
            Assert.IsTrue(await _memDb.SetMoveAsync("set1", "set2", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(3, CallsByMemDb);

            Assert.IsTrue(await _memDb.SetContainsAsync("set2", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetMoveAsync_InRedis()
        {
            Assert.IsTrue(await _redisDirectDb.SetAddAsync("set1", "value1"));
            Assert.IsTrue(await _memDb.SetMoveAsync("set1", "set2", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsTrue(await _memDb.SetContainsAsync("set2", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }

        [TestMethod]
        public async Task SetMoveAsync_ByOtherClient()
        {
            Assert.IsTrue(await _memDb.SetAddAsync("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(await _otherClientDb.SetMoveAsync("set1", "set2", "value1"));
            await Task.Delay(50);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            Assert.IsTrue(await _memDb.SetContainsAsync("set2", "value1"));
            Assert.AreEqual(3, CallsByMemDb);
        }
    }
}