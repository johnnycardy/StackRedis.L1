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
        public void HashIncrement_Double()
        {
            _memDb.HashSet("hashKey", "key1", "1");
            _memDb.HashIncrement("hashKey", "key1", 1.0);
            Assert.AreEqual("2", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public void HashIncrement_Int()
        {
            _memDb.HashSet("hashKey", "key1", "1");
            _memDb.HashIncrement("hashKey", "key1", 1);
            Assert.AreEqual("2", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public void HashDecrement_Double()
        {
            _memDb.HashSet("hashKey", "key1", "2");
            _memDb.HashDecrement("hashKey", "key1", 1.0);
            Assert.AreEqual("1", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public void HashDecrement_Int()
        {
            _memDb.HashSet("hashKey", "key1", "2");
            _memDb.HashDecrement("hashKey", "key1", 1);
            Assert.AreEqual("1", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashIncrement_OtherClient_Double()
        {
            _memDb.HashSet("hashKey", "key1", "1");
            _otherClientDb.HashIncrement("hashKey", "key1", 1.0);
            await Task.Delay(50);
            Assert.AreEqual("2", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashIncrement_OtherClient_Int()
        {
            _memDb.HashSet("hashKey", "key1", "1");
            _otherClientDb.HashIncrement("hashKey", "key1", 1);
            await Task.Delay(50);
            Assert.AreEqual("2", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashDecrement_OtherClient_Double()
        {
            _memDb.HashSet("hashKey", "key1", "2");
            _otherClientDb.HashDecrement("hashKey", "key1", 1.0);
            await Task.Delay(50);
            Assert.AreEqual("1", (string)_memDb.HashGet("hashKey", "key1"));
        }

        [TestMethod]
        public async Task HashDecrement_OtherClient_Int()
        {
            _memDb.HashSet("hashKey", "key1", "2");
            _otherClientDb.HashDecrement("hashKey", "key1", 1);
            await Task.Delay(50);
            Assert.AreEqual("1", (string)_memDb.HashGet("hashKey", "key1"));
        }
    }
}
