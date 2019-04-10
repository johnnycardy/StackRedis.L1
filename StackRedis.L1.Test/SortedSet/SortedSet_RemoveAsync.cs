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
    public class SortedSet_RemoveAsync : UnitTestBase
    {
        [TestMethod]
        public async Task SortedSet_RemoveAsync_Simple()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1)
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, CallsByMemDb); //Should be cached

            await _memDb.SortedSetRemoveAsync("key", "mem1");
            Assert.AreEqual(2, CallsByMemDb);

            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(3, CallsByMemDb); //Should go to the server to confirm it's gone
        }

        [TestMethod]
        public async Task SortedSet_Remove_ByOtherClient()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1)
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            var result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, CallsByMemDb); //Should be cached

            await _otherClientDb.SortedSetRemoveAsync("key", "mem1");

            //Wait for it to propagate...
            await Task.Delay(50);

            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Should go to the server to confirm it's gone
        }

        [TestMethod]
        public async Task SortedSet_Remove_ByOtherClient_MiddleOfRange()
        {
            await _memDb.SortedSetAddAsync("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3)
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);
            
            var result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //not cached yet

            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Now it's cached
            
            await _otherClientDb.SortedSetRemoveAsync("key", "mem2");

            //Wait for it to propagate...
            await Task.Delay(50);
            
            result = await _memDb.SortedSetRangeByScoreAsync("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Should not need to go to server
        }
    }
}
