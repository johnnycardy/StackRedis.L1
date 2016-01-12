using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetRemove : UnitTestBase
    {
        [TestMethod]
        public void SetRemove_Simple()
        {
            _redisDirectDb.SetAdd("set1", "value1");
            Assert.IsTrue(_memDb.SetRemove("set1", "value1"));
            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            //Ensure it's gone
            Assert.IsFalse(_redisDirectDb.SetContains("set1", "value1"));

            //Try and remove it again
            Assert.IsFalse(_memDb.SetRemove("set1", "value1"));
        }

        [TestMethod]
        public void SetRemove_KeyNotPresent()
        {
            Assert.IsFalse(_memDb.SetRemove("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            //Ensure it's still not there
            Assert.IsFalse(_redisDirectDb.SetContains("set1", "value1"));
        }
        
        [TestMethod]
        public async Task SetRemove_OtherClient_String()
        {
            _memDb.SetAdd("set1", "value1");
            Assert.IsTrue(_memDb.SetContains("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.SetRemove("set1", "value1");

            //Wait for it to propagate
            await Task.Delay(50);

            Assert.IsFalse(_memDb.SetContains("set1", "value1"));
        }

        [TestMethod]
        public async Task SetRemove_OtherClient_Int()
        {
            _memDb.SetAdd("set1", 1);
            Assert.IsTrue(_memDb.SetContains("set1", 1));
            Assert.AreEqual(1, CallsByMemDb);

            _otherClientDb.SetRemove("set1", 1);

            //Wait for it to propagate
            await Task.Delay(50);

            Assert.IsFalse(_memDb.SetContains("set1", 1));
        }

        [TestMethod]
        public async Task SetRemoveAsync_Simple()
        {
            await _redisDirectDb.SetAddAsync("set1", "value1");
            Assert.IsTrue(await _memDb.SetRemoveAsync("set1", "value1"));
            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(2, CallsByMemDb);

            //Ensure it's gone
            Assert.IsFalse(await _redisDirectDb.SetContainsAsync("set1", "value1"));

            //Try and remove it again
            Assert.IsFalse(await _memDb.SetRemoveAsync("set1", "value1"));
        }

        [TestMethod]
        public async Task SetRemoveAsync_KeyNotPresent()
        {
            Assert.IsFalse(await _memDb.SetRemoveAsync("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            //Ensure it's still not there
            Assert.IsFalse(await _redisDirectDb.SetContainsAsync("set1", "value1"));
        }

        [TestMethod]
        public async Task SetRemoveAsync_OtherClient()
        {
            await _memDb.SetAddAsync("set1", "value1");
            Assert.IsTrue(await _memDb.SetContainsAsync("set1", "value1"));
            Assert.AreEqual(1, CallsByMemDb);

            await _otherClientDb.SetRemoveAsync("set1", "value1");

            //Wait for it to propagate
            await Task.Delay(50);

            Assert.IsFalse(await _memDb.SetContainsAsync("set1", "value1"));
        }
    }
}