using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_RangeByScore : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_RangeByScore_CanMarkAsContinuous()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.SortedSetRangeByScore("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //The cache database doesn't know that they are continuous.

            result = _memDb.SortedSetRangeByScore("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Now the cache DB does know that they are continuous.
        }

        [TestMethod]
        public void SortedSet_RangeByScore_CannotMarkAsContinuous()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });
            Assert.AreEqual(1, CallsByMemDb);

            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem4", 4)
            });

            var result = _memDb.SortedSetRangeByScore("key", 1, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //The cache database doesn't know that 1 and 2 are continuous.

            result = _memDb.SortedSetRangeByScore("key", 1, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(3, CallsByMemDb); //Now the cache DB still doesn't know that 1 and 2 are continuous.

            //However the cache DB should know that 1, 2, 3 are continuous
            result = _memDb.SortedSetRangeByScore("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(3, CallsByMemDb);

            //Ensure there is only a single value with score '1'
            var val = _memDb.SortedSetRangeByScore("key", 1, 1);
            Assert.AreEqual("mem1", (string)val.First());

            _memDb.SortedSetAdd("key", "mem1", 1.5);
            val = _memDb.SortedSetRangeByScore("key", 1, 1.5);
            Assert.AreEqual("mem1", (string)val.Single()); //There should be no element with value 1
        }

        //select range between 0 - 10, items 2 - 7 returned, but 0-10 is cached for next time (Maybe with dummy items?)
        [TestMethod]
        public void SortedSet_RangeByScore_CachingOutsideRange()
        {
            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });

            var result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 4);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //Call again - it should be cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 3);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //Add another item inside this cached range
            _memDb.SortedSetAdd("key", "mem0", 0);
            Assert.AreEqual(2, CallsByMemDb);

            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 3);
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public void SortedSet_RangeByInfinity()
        {
            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });
            var result = _memDb.SortedSetRangeByScoreWithScores("key");
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            result = _memDb.SortedSetRangeByScoreWithScores("key");
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, CallsByMemDb);
        }

        [TestMethod]
        public void SortedSet_RangeEmpty()
        {
            var result = _memDb.SortedSetRangeByScoreWithScores("key");
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            result = _memDb.SortedSetRangeByScoreWithScores("key");
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(2, CallsByMemDb);
        }

        //When 'take' is specified, the upper bound needs to be ignored with regard to caching.
        [TestMethod]
        public void SortedSet_RangeInfinity_TakeSubset()
        {
            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
               {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
               });

            var result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, 2);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //First page of 2 should be cached....
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, 2);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //First page of 100 should not be cached...
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb);

            //Now it should be
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb);
        }

        [TestMethod]
        public void SortedSet_RangeInfinity_Skip()
        {
            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
               {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
               });

            //skip 1 take all
            var result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 1);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, CallsByMemDb);

            //Can't be cached since skip was nonzero
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 1);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb);

            //Skip 0 this time and take 2
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, 2);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, CallsByMemDb);

            //Should be cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, 2);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, CallsByMemDb);

            //skip 2 and take 1 - should be cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 2, 1);
            Assert.AreEqual(1, result.Count());
             Assert.AreEqual(3, CallsByMemDb);

            //Skip 0 and take 3 - should be cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, 3);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(3, CallsByMemDb);

            //Retrieve all between 0 and 100 - should be cached
            result = _memDb.SortedSetRangeByScoreWithScores("key", 0, 100, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(3, CallsByMemDb);
        }
    }
}
