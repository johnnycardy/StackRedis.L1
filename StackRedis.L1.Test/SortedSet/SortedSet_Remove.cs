using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_Remove : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_Remove_Simple()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1)
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.SortedSetRangeByScore("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, CallsByMemDb); //Should be cached

            _memDb.SortedSetRemove("key", "mem1");
            Assert.AreEqual(2, CallsByMemDb);

            result = _memDb.SortedSetRangeByScore("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(3, CallsByMemDb); //Should go to the server to confirm it's gone
        }

        [TestMethod]
        public async Task SortedSet_Remove_ByOtherClient()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1)
            });
            Assert.AreEqual(1, CallsByMemDb);

            var result = _memDb.SortedSetRangeByScore("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, CallsByMemDb); //Should be cached

            _otherClientDb.SortedSetRemove("key", "mem1");

            //Wait for it to propagate...
            await Task.Delay(50);

            result = _memDb.SortedSetRangeByScore("key", 1, 1, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Should go to the server to confirm it's gone
        }

        [TestMethod]
        public async Task SortedSet_Remove_ByOtherClient_MiddleOfRange()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3)
            });
            Assert.AreEqual(1, CallsByMemDb);
            
            var result = _memDb.SortedSetRangeByScore("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //not cached yet

            result = _memDb.SortedSetRangeByScore("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Now it's cached
            
            _otherClientDb.SortedSetRemove("key", "mem2");

            //Wait for it to propagate...
            await Task.Delay(50);
            
            result = _memDb.SortedSetRangeByScore("key", 1, 3, StackExchange.Redis.Exclude.None);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, CallsByMemDb); //Should not need to go to server
        }
    }
}
