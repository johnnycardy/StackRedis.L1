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
    public class SortedSet_RemoveRangeByScoreAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_RemoveRangeByScoreAsync_RangeFullyCached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = await _memDb.SortedSetRemoveRangeByScoreAsync("key", 1, 2);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, result);

            range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2);
            Assert.AreEqual(0, range.Count()); //Ensure it's removed
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByScoreAsync_RangePartlyCached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = await _memDb.SortedSetRemoveRangeByScoreAsync("key", 1, 3);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, result);

            range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2);
            Assert.AreEqual(0, range.Count()); //Ensure it's removed
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByScoreAsync_RangeDisjointedCached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            
            long result = await _memDb.SortedSetRemoveRangeByScoreAsync("key", 1, 3);
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(2, result);

            Assert.IsFalse((await _memDb.SortedSetScoreAsync("key", "mem1")).HasValue);
            Assert.IsFalse((await _memDb.SortedSetScoreAsync("key", "mem2")).HasValue);
        }

        [TestMethod]
        public async Task SortedSet_RemoveRangeByScoreAsync_OtherClient()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = await _otherClientDb.SortedSetRemoveRangeByScoreAsync("key", 1, 2);
            Assert.AreEqual(2, result);

            await Task.Delay(50);

            range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2);
            Assert.AreEqual(0, range.Count()); //Ensure it's removed
        }


        [TestMethod]
        public async Task SortedSet_RemoveRangeByScoreAsync_OtherClient_ExcludeStop()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            var range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2); //So that it's registered as a range
            Assert.AreEqual(2, range.Count());
            Assert.AreEqual(2, CallsByMemDb);

            long result = await _otherClientDb.SortedSetRemoveRangeByScoreAsync("key", 1, 2, StackExchange.Redis.Exclude.Stop);
            Assert.AreEqual(1, result);

            await Task.Delay(50);

            range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 2);
            Assert.AreEqual(1, range.Count()); //Ensure one item is removed
        }
    }
}
