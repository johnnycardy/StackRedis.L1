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
    public class SortedSet_RemoveRangeByScore : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_RemoveRangeByScore_RangeFullyCached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = _memDb.SortedSetRangeByScore("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = _memDb.SortedSetRemoveRangeByScore("key", 1, 2);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, result);

            range = _memDb.SortedSetRangeByScore("key", 1, 2);
            Assert.AreEqual(0, range.Count()); //Ensure it's removed
        }

        [TestMethod]
        public void SortedSet_RemoveRangeByScore_RangePartlyCached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = _memDb.SortedSetRangeByScore("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = _memDb.SortedSetRemoveRangeByScore("key", 1, 3);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, result);

            range = _memDb.SortedSetRangeByScore("key", 1, 2);
            Assert.AreEqual(0, range.Count()); //Ensure it's removed
        }

        [TestMethod]
        public void SortedSet_RemoveRangeByScore_RangeDisjointedCached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            
            long result = _memDb.SortedSetRemoveRangeByScore("key", 1, 3);
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(2, result);

            Assert.IsFalse(_memDb.SortedSetScore("key", "mem1").HasValue);
            Assert.IsFalse(_memDb.SortedSetScore("key", "mem2").HasValue);
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByScore_OtherClient()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = _memDb.SortedSetRangeByScore("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = _otherClientDb.SortedSetRemoveRangeByScore("key", 1, 2);
            Assert.AreEqual(2, result);

            await Task.Delay(50);

            range = _memDb.SortedSetRangeByScore("key", 1, 2);
            Assert.AreEqual(0, range.Count()); //Ensure it's removed
        }


        [TestMethod]
        public async Task SortedSet_RemoveRangeByScore_OtherClient_ExcludeStop()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = _memDb.SortedSetRangeByScore("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = _otherClientDb.SortedSetRemoveRangeByScore("key", 1, 2, StackExchange.Redis.Exclude.Stop);
            Assert.AreEqual(1, result);

            await Task.Delay(50);

            range = _memDb.SortedSetRangeByScore("key", 1, 2);
            Assert.AreEqual(1, range.Count()); //Ensure one item is removed
        }
    }
}
