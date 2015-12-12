using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class Keyspace_rename : UnitTestBase
    {
        [TestMethod]
        public async Task Keyspace_rename_Simple()
        {

            //Create a key
            _redisDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));

            //Rename it in Redis
            _redisDb.KeyRename("key1", "renamedKey1");

            //Wait 100ms for it to take effect
            await Task.Delay(100);

            //Ensure it's gone from the memory db
            Assert.IsFalse(_memDb.KeyExists("key1"));
            Assert.IsTrue(_memDb.KeyExists("renamedKey1"));
        }
        
    }
}
