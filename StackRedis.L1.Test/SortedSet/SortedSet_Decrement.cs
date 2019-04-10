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
    public class SortedSet_Decrement : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_Decrement_Cached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.SortedSetDecrement("key", "mem2", 0.5);
            Assert.AreEqual(2, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", "mem2"); //Shouldn't need to go to server
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(1.5, score.Value);
        }


        [TestMethod]
        public void SortedSet_Decrement_OldScoreRemoved()
        {
            _memDb.SortedSetAdd("key", "mem1", 10, When.Always);
            _memDb.SortedSetDecrement("key", "mem1", 1);
            Assert.AreEqual(9, _memDb.SortedSetScore("key", "mem1").Value);
            Assert.IsFalse(_memDb.SortedSetRangeByScore("key", 10, 10).Any());
        }

        [TestMethod]
        public void SortedSet_Decrement_WithinRange_Cached()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
                new StackExchange.Redis.SortedSetEntry("mem3", 3),
            }, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            _memDb.SortedSetDecrement("key", "mem2", 1.5);
            Assert.AreEqual(2, CallsByMemDb);

            var range = _memDb.SortedSetRangeByScore("key", 1, 3);

            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, range.Length); //Should only have items 1 and 3 in it, and does need a server call 

            range = _memDb.SortedSetRangeByScore("key", 1, 3);
            Assert.AreEqual(3, CallsByMemDb);
            Assert.AreEqual(2, range.Length); //Should only have items 1 and 3 in it, and doesn't need a server call 

            range = _memDb.SortedSetRangeByScore("key", 0, 3); //Needs a server request
            Assert.AreEqual(4, CallsByMemDb);
            Assert.AreEqual(3, range.Length);
        }

        [TestMethod]
        public void SortedSet_Decrement_NotCached()
        {

            _otherClientDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            Assert.AreEqual(0, CallsByMemDb);

            _memDb.SortedSetDecrement("key", "mem2", 0.5);
            Assert.AreEqual(1, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", "mem2"); //needs to go to server
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(1.5, score.Value);
        }

        [TestMethod]
        public async Task SortedSet_Decrement_OtherClient()
        {
            _memDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            }, When.Always);
            
            _otherClientDb.SortedSetDecrement("key", "mem2", 0.5);

            await Task.Delay(50); //Wait for it to propagate

            Assert.AreEqual(1, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", "mem2"); //needs to go to server
            Assert.AreEqual(2, CallsByMemDb);
            Assert.AreEqual(1.5, score.Value);
        }
    }
}
