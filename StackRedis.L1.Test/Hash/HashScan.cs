using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;
using System.Linq;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class HashScan : UnitTestBase
    {
        [TestMethod]
        public void HashScan_StoresInMem()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(0, CallsByMemDb);

            var result =_memDb.HashScan("hashKey", "key*", 10, CommandFlags.None);
            var item = result.First();
            Assert.AreEqual(1, CallsByMemDb);

            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual("key1", (string)item.Name);
            Assert.AreEqual("value1", (string)item.Value);

            var key1 = _memDb.HashGet("hashKey", "key1");
            Assert.AreEqual("value1", (string)key1);
            Assert.AreEqual(1, CallsByMemDb);

            //Key2 shouldn't yet be in memory since we only called 'First' on the HashScan IEnumerable
            var key2 = _memDb.HashGet("hashKey", "key2");
            Assert.AreEqual("value2", (string)key2);
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public void HashScan_StoresInMem_ContinueEnumeration()
        {
            _redisDirectDb.HashSet("hashKey", new HashEntry[] { new HashEntry("key1", "value1"), new HashEntry("key2", "value2") });
            Assert.AreEqual(0, CallsByMemDb);

            var result = _memDb.HashScan("hashKey", "key*", 10, CommandFlags.None);
            var item = result.ToArray().First();
            Assert.AreEqual(1, CallsByMemDb);

            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual("key1", (string)item.Name);
            Assert.AreEqual("value1", (string)item.Value);

            var key1 = _memDb.HashGet("hashKey", "key1");
            Assert.AreEqual("value1", (string)key1);
            Assert.AreEqual(1, CallsByMemDb);
            
            //Key2 should also be in memory since ToArray was called
            var key2 = _memDb.HashGet("hashKey", "key2");
            Assert.AreEqual("value2", (string)key2);
            Assert.AreEqual(1, CallsByMemDb);
        }
    }
}
