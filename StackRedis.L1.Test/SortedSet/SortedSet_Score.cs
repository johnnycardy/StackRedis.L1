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
    public class SortedSet_Score : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_Score_Cached()
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
        }

        [TestMethod]
        public void SortedSet_Score_NotCached()
        {
            _redisDirectDb.SortedSetAdd("key", new StackExchange.Redis.SortedSetEntry[]
            {
                new StackExchange.Redis.SortedSetEntry("mem1", 1),
                new StackExchange.Redis.SortedSetEntry("mem2", 2),
            });
            Assert.AreEqual(0, CallsByMemDb);

            double? score = _memDb.SortedSetScore("key", "mem2");
            Assert.AreEqual(1, CallsByMemDb); //Had to go to server
            Assert.AreEqual(2, score.Value);
            
            score = _memDb.SortedSetScore("key", "mem2");
            Assert.AreEqual(1, CallsByMemDb); //Didn't need to go to server
            Assert.AreEqual(2, score.Value);

            //Now the whole entry is cached, too
            var range = _memDb.SortedSetRangeByScore("key", 2, 2);
            Assert.AreEqual(1, range.Count());
            Assert.AreEqual(1, CallsByMemDb);
        }
    }
}
