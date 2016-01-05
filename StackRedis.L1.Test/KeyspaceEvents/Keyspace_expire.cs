using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class Keyspace_expire : UnitTestBase
    {
        [TestMethod]
        public async Task Keyspace_expire_KeyRemoved()
        {
            //Create a key
            _redisDirectDb.StringSet("key1", "value1");
            Assert.AreEqual("value1", (string)_memDb.StringGet("key1"));

            //Expire it in Redis
            _otherClientDb.KeyExpire("key1", TimeSpan.FromMilliseconds(10));

            //Wait for it to take effect
            await Task.Delay(200);

            //Ensure it's gone from the memory db
            Assert.IsFalse(_memDb.KeyExists("key1"));
        }
    }
}
