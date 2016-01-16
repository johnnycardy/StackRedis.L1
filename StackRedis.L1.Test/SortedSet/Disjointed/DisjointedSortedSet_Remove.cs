using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache.Types.SortedSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class DisjointedSortedSet_Remove
    {
        [TestMethod]
        public void DisjointedSortedSet_Remove_Simple()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 20)
            });
            Assert.AreEqual(2, set.Count);
            set.Remove(new RedisValue[] { "anna" });
            Assert.AreEqual(1, set.Count);
        }

        [TestMethod]
        public void DisjointedSortedSet_Remove_InMiddle()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 20),
                new StackExchange.Redis.SortedSetEntry("carol", 30)
            });
            set.Remove(new RedisValue[] { "bob" });
            var result = set.RetrieveByScore(10, 30, Exclude.None);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void DisjointedSortedSet_Remove_Multiple()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 20),
                new StackExchange.Redis.SortedSetEntry("carol", 30),
                new StackExchange.Redis.SortedSetEntry("derek", 40),
                new StackExchange.Redis.SortedSetEntry("errol", 50)
            });
            set.Remove(new RedisValue[] { "bob", "derek" });
            var result = set.RetrieveByScore(10, 50, Exclude.None);
            Assert.AreEqual(3, result.Count());
        }
    }
}
