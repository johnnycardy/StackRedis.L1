using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_Add : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_Add_Single()
        {
            _memDb.SortedSetAdd("key", "member", 1);
            Assert.AreEqual(1, CallsByMemDb);
            var result = _memDb.SortedSetRangeByScore("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void SortedSet_Add_Multiple()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[] 
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
        }


        [TestMethod]
        public void SortedSet_Add_Multiple_Reverse()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void SortedSet_Add_RedisDirect()
        {
            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
            });
            var result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //Check that it's cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public async Task SortedSet_Add_OtherClient()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });

            //Warm up the mem cache
            var result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb);

            //Change the scores with another client and make sure it's updated
            _otherClientDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 3),
                new StackExchange.Redis.SortedSetEntry("mem2", 4),
            });

            await Task.Delay(50);

            //It should go back to the server for the new values
            result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            result = _memDb.SortedSetRangeByScoreWithScores("key", 3, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(4, CallsByMemDb);

            //Check that it's cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(5, CallsByMemDb);
            result = _memDb.SortedSetRangeByScoreWithScores("key", 3, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(5, CallsByMemDb);
        }
    }
}
