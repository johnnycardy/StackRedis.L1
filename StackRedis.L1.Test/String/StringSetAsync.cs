using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringSetAsync : UnitTestBase
    {
        [TestMethod]
        public async Task StringSetAsync_ThenGet()
        {
            await _memDb.StringSetAsync("key1", "value1");
            Assert.AreEqual(1, _redisDb.Calls);
            
            //value1 should be mem cached
            Assert.AreEqual("value1", (string)(await _memDb.StringGetAsync("key1")));
            Assert.AreEqual(1, _redisDb.Calls);
        }


        [TestMethod]
        public async Task StringSetAsyncMulti_ThenGet()
        {
            await _memDb.StringSetAsync(new[]
            {
                new System.Collections.Generic.KeyValuePair<RedisKey, RedisValue>("key1", "value1"),
                new System.Collections.Generic.KeyValuePair<RedisKey, RedisValue>("key2", "value2"),
            });

            Assert.AreEqual(1, _redisDb.Calls);

            //remove key1 in redis only - not in memory
            _memDb.PauseKeyspaceNotifications(true);
            await _redisDb.KeyDeleteAsync("key1");

            //key1 should be mem cached
            var result = await _memDb.StringGetAsync(new RedisKey[] { "key1", "key2" });
            Assert.AreEqual("value1", (string)result[0]);
            Assert.AreEqual("value2", (string)result[1]);
        }


    }
}
