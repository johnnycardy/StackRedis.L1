using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashIncrementAsync : UnitTestBase
    {
        [TestMethod]
        public async Task HashIncrementAsync_Double()
        {
            await _memDb.HashSetAsync("hashKey", "key1", "1");
            await _otherClientDb.HashIncrementAsync("hashKey", "key1", 1.0);
            await Task.Delay(50);
            Assert.AreEqual("2", (string)await _memDb.HashGetAsync("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashIncrementAsync_Int()
        {
            await _memDb.HashSetAsync("hashKey", "key1", "1");
            await _otherClientDb.HashIncrementAsync("hashKey", "key1", 1);
            await Task.Delay(50);
            Assert.AreEqual("2", (string)await _memDb.HashGetAsync("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashDecrementAsync_Double()
        {
            await _memDb.HashSetAsync("hashKey", "key1", "2");
            await _otherClientDb.HashDecrementAsync("hashKey", "key1", 1.0);
            await Task.Delay(50);
            Assert.AreEqual("1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashDecrementAsync_Int()
        {
            await _memDb.HashSetAsync("hashKey", "key1", "2");
            await _otherClientDb.HashDecrementAsync("hashKey", "key1", 1);
            await Task.Delay(50);
            Assert.AreEqual("1", (string)await _memDb.HashGetAsync("hashKey", "key1"));
        }
    }
}
