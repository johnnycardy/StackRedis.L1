using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashIncrement : UnitTestBase
    {
        [TestMethod]
        public async Task HashIncrement_Double()
        {
            _memDb.HashSet("hashKey", "key1", "1");
            _otherClientDb.HashIncrement("hashKey", "key1", 1.0);
            await Task.Delay(50);
            Assert.AreEqual("2", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashIncrement_Int()
        {
            _memDb.HashSet("hashKey", "key1", "1");
            _otherClientDb.HashIncrement("hashKey", "key1", 1);
            await Task.Delay(50);
            Assert.AreEqual("2", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashDecrement_Double()
        {
            _memDb.HashSet("hashKey", "key1", "2");
            _otherClientDb.HashDecrement("hashKey", "key1", 1.0);
            await Task.Delay(50);
            Assert.AreEqual("1", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashDecrement_Int()
        {
            _memDb.HashSet("hashKey", "key1", "2");
            _otherClientDb.HashDecrement("hashKey", "key1", 1);
            await Task.Delay(50);
            Assert.AreEqual("1", (string)_memDb.HashGet("hashKey", "key1"));
        }
    }
}
