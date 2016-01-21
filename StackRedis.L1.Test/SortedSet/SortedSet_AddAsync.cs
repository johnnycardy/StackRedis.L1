using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_AddAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_AddAsync_Single()
        {
            await _memDb.SortedSetAddAsync("key", "member", 1);
            Assert.AreEqual(1, CallsByMemDb);
            var result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task SortedSet_AddAsync_Multiple()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[] 
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
        }


        [TestMethod]
        public async Task SortedSet_AddAsync_Multiple_Reverse()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task SortedSet_AddAsync_RedisDirect()
        {
            await _redisDirectDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
            });
            var result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //Check that it's cached
            result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task SortedSet_AddAsync_OtherClient()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });

            //Warm up the mem cache
            var result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb);

            //Change the scores with another client and make sure it's updated
            await _otherClientDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 3),
                new StackExchange.Redis.SortedSetEntry("mem2", 4),
            });

            await Task.Delay(50);

            //It should go back to the server for the new values
            result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 3, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(4, CallsByMemDb);

            //Check that it's cached
            result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(5, CallsByMemDb);
            result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 3, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(5, CallsByMemDb);
        }
    }
}
