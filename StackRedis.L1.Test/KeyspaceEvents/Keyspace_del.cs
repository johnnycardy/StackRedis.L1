using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class Keyspace_del : UnitTestBase
    {
        [TestMethod]
        public async Task Keyspace_del_KeyRemoved()
        {
            _memDb.DBData.Listener.Paused = false;

            //Create a key
            _redisDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));

            //Delete it in Redis
            _redisDb.KeyDelete("key1");

            //Wait 100ms for it to take effect
            await Task.Delay(100);

            //Ensure it's gone from the memory db
            Assert.IsFalse(_memDb.KeyExists("key1"));
        }
    }
}
