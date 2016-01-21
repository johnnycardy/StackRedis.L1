using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_RangeByScoreAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_RangeByScoreAsync_CanMarkAsContinuous()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //The cache database doesn't know that they are continuous.

            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Now the cache DB does know that they are continuous.
        }

        [TestMethod]
        public async Task SortedSet_RangeByScoreAsync_CannotMarkAsContinuous()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });
            Assert.AreEqual(1, CallsByMemDb);

            await _redisDirectDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem4", 4)
            });

            var result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //The cache database doesn't know that 1 and 2 are continuous.

            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 4, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(3, CallsByMemDb); //Now the cache DB still doesn't know that 1 and 2 are continuous.

            //However the cache DB should know that 1, 2, 3 are continuous
            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(3, CallsByMemDb);

            //Ensure there is only a single value with score '1'
            var val = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1);
            Assert.AreEqual("mem1", (string)val.First());

            await _memDb.SortedSetAddAsync("key", "mem1", 1.5);
            val = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1.5);
            Assert.AreEqual("mem1", (string)val.Single()); //There should be no element with value 1
        } 
    }
}
