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
    public class SortedSet_RemoveRangeByValue : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_RemoveRangeByValue_Cached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry(1, 1),
                new StackExchange.Redis.SortedSetEntry(2, 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", 2);
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            _memDb.SortedSetRemoveRangeByValue("key", 2, 2);
            Assert.AreEqual(2, CallsByMemDb);

            score = _memDb.SortedSetScore("key", 2);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByValue_OtherClient()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry(1, 1),
                new StackExchange.Redis.SortedSetEntry(2, 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", 2);
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual(2, score.Value);

            _otherClientDb.SortedSetRemoveRangeByValue("key", 2, 2);

            await Task.Delay(50);

            score = _memDb.SortedSetScore("key", 2);
            Assert.AreEqual(2, CallsByMemDb);
            Assert.IsFalse(score.HasValue);
        }
    }
}
