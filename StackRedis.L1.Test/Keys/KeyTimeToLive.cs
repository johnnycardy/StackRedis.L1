using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyTimeToLive : UnitTestBase
    {

        [TestMethod]
        public void KeyTimeToLive_Simple()
        {
            _memDb.StringSet("key1", "value1");

            //Give it a ttl in redis
            _redisDirectDb.KeyExpire("key1", TimeSpan.FromMinutes(10));

            //Get the ttl via mem
            TimeSpan? ttl = _memDb.KeyTimeToLive("key1");
            Assert.IsTrue(ttl.HasValue);
            Assert.AreEqual(2, CallsByMemDb);

            //Check the ttl in the in-memory database without going to redis
            ttl = _memDb.KeyTimeToLive("key1");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.IsTrue(ttl.Value.TotalMinutes > 9 && ttl.Value.TotalMinutes < 10);
        }

        [TestMethod]
        public async Task KeyTimeToLive_ByOtherClient()
        {
            _memDb.StringSet("key1", "value1");

            //Give it a ttl in redis
            _redisDirectDb.KeyExpire("key1", TimeSpan.FromMinutes(10));

            //Get the ttl via mem
            TimeSpan? ttl = _memDb.KeyTimeToLive("key1");
            Assert.IsTrue(ttl.HasValue);
            Assert.AreEqual(2, CallsByMemDb);

            //Set the value from another client
            _otherClientDb.KeyExpire("key1", TimeSpan.FromMinutes(5));

            //Give it a moment to propagate
            await Task.Delay(50);

            //Check the ttl in the in-memory database without going to redis
            ttl = _memDb.KeyTimeToLive("key1");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.IsTrue(ttl.Value.TotalMinutes > 4 && ttl.Value.TotalMinutes < 5);
        }
    }
}
