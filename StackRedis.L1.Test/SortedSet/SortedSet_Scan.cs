using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_Scan : UnitTestBase
    {
        [TestMethod]
        public void SortedSetScan_AddsToCache()
        {
            _redisDirectDb.SortedSetAdd("key", "mem1", 1);
            var scanResult = _memDb.SortedSetScan("key", "mem1");
            Assert.AreEqual(1, scanResult.Single().Score);
            Assert.AreEqual(1, CallsByMemDb);

            //Should now be cached
            var byScoreResult = _memDb.SortedSetRangeByScore("key", 1, 1);
            Assert.AreEqual("mem1", (string)byScoreResult.Single());
            Assert.AreEqual(1, CallsByMemDb);
        }


        [TestMethod]
        public void SortedSetScan_AddsToCache_Overload2()
        {
            _redisDirectDb.SortedSetAdd("key", "mem1", 1);
            var scanResult = _memDb.SortedSetScan("key", "mem1", 10, StackExchange.Redis.CommandFlags.HighPriority);
            Assert.AreEqual(1, scanResult.Single().Score);
            Assert.AreEqual(1, CallsByMemDb);

            //Should now be cached
            var byScoreResult = _memDb.SortedSetRangeByScore("key", 1, 1);
            Assert.AreEqual("mem1", (string)byScoreResult.Single());
            Assert.AreEqual(1, CallsByMemDb);
        }
    }
}
