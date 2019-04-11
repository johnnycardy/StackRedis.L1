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
    public class SortedSet_RemoveRangeByRank : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_RemoveRangeByRank_Cached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", "mem2");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            _memDb.SortedSetRemoveRangeByRank("key", 1, 1); //0-based index
            Assert.AreEqual(2, CallsByMemDb);

            score = _memDb.SortedSetScore("key", "mem2");
            Assert.AreEqual(3, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByRank_OtherClient()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", "mem2");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            _otherClientDb.SortedSetRemoveRangeByRank("key", 1, 1); //0-based index

            await Task.Delay(50);

            score = _memDb.SortedSetScore("key", "mem2");
            Assert.AreEqual(2, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }
    }
}
