using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_RangeByScoreWithScoresAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_RangeByScoreWithScoresAsync_Cached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //The cache database doesn't know that 1 and 2 are continuous.

            result = await _memDb.SortedSetRangeByScoreWithScoresAsync("key", 1, 2, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Now the cache DB does know that 1 and 2 are continuous.
        }

    }
}
