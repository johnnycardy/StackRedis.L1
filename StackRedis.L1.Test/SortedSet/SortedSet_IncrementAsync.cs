using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_IncrementAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_IncrementAsync_Cached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.SortedSetIncrementAsync("key", "mem2", 0.5);
            Assert.AreEqual(2, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", "mem2"); //Shouldn't need to go to server
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(2.5, score.Value);
        }


        [TestMethod]
        public async Task SortedSet_IncrementAsync_OldScoreRemoved()
        {
            await _memDb.SortedSetAddAsync("key", "mem1", 10);
            await _memDb.SortedSetIncrementAsync("key", "mem1", 1);
            Assert.AreEqual(11, (await _memDb.SortedSetScoreAsync("key", "mem1")).Value);
            Assert.IsFalse((await _memDb.SortedSetRangeByScoreAsync("key", 10, 10)).Any());
        }

        [TestMethod]
        public async Task SortedSet_IncrementAsync_WithinRange_Cached()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            });
            Assert.AreEqual(1, CallsByMemDb);

            await _memDb.SortedSetIncrementAsync("key", "mem2", 1.5);
            Assert.AreEqual(2, CallsByMemDb);

            var range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3);

            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, range.Length); //Should only have items 1 and 3 in it, and does need a server call 

            range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, range.Length); //Should only have items 1 and 3 in it, and doesn't need a server call 

            range = await _memDb.SortedSetRangeByScoreAsync("key", 1, 4); //Needs a server request
            Assert.AreEqual(4, CallsByMemDb);
            Assert.AreEqual(3, range.Length);
        }

        [TestMethod]
        public async Task SortedSet_IncrementAsync_NotCached()
        {
            await _otherClientDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            Assert.AreEqual(0, CallsByMemDb);

            await _memDb.SortedSetIncrementAsync("key", "mem2", 0.5);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", "mem2"); //needs to go to server
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(2.5, score.Value);
        }

        [TestMethod]
        public async Task SortedSet_Increment_OtherClient()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            
            await _otherClientDb.SortedSetIncrementAsync("key", "mem2", 0.5);

            await Task.Delay(50); //Wait for it to propagate

            Assert.AreEqual(1, CallsByMemDb);

            double? score = await _memDb.SortedSetScoreAsync("key", "mem2"); //needs to go to server
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(2.5, score.Value);
        }
    }
}
