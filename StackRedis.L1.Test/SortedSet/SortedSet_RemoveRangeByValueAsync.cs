using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_RemoveRangeByValueAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_RemoveRangeByValueAsync_Cached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry(1, 1),
                new StackExchange.Redis.SortedSetEntry(2, 2),
            });
            Assert.AreEqual(1, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", 2);
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            await _memDb.SortedSetRemoveRangeByValueAsync("key", 2, 2);
            Assert.AreEqual(2, CallsByMemDb);

            score = await _memDb.SortedSetScoreAsync("key", 2);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByValueAsync_OtherClient()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry(1, 1),
                new StackExchange.Redis.SortedSetEntry(2, 2),
            });
            Assert.AreEqual(1, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", 2);
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            await _otherClientDb.SortedSetRemoveRangeByValueAsync("key", 2, 2);

            await Task.Delay(50);

            score = await _memDb.SortedSetScoreAsync("key", 2);
            Assert.AreEqual(2, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }
    }
}
