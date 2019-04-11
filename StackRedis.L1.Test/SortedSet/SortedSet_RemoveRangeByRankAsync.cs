using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_RemoveRangeByRankAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_RemoveRangeByRankAsync_Cached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", "mem2");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            await _memDb.SortedSetRemoveRangeByRankAsync("key", 1, 1); //0-based index
            Assert.AreEqual(2, CallsByMemDb);

            score = await _memDb.SortedSetScoreAsync("key", "mem2");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByRank_OtherClient()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", "mem2");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            await _otherClientDb.SortedSetRemoveRangeByRankAsync("key", 1, 1); //0-based index

            await Task.Delay(50);

            score = await _memDb.SortedSetScoreAsync("key", "mem2");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }
    }
}
