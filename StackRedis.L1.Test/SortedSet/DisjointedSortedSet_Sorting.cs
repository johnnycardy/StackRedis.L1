using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackRedis.L1.MemoryCache.Types.SortedSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class DisjointedSortedSet_Sorting
    {
        [TestMethod]
        public void DisjointedSortedSet_Sorting_Simple()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 20)
            });
            var result = set.RetrieveByScore(10, 20);
            Assert.AreEqual(10, result.ElementAt(0).Score);
            Assert.AreEqual(20, result.ElementAt(1).Score);
        }

        [TestMethod]
        public void DisjointedSortedSet_Sorting_Reverse()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 20)
            });

            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("carol", 15),
                new StackExchange.Redis.SortedSetEntry("derek", 25)
            });

            var result = set.RetrieveByScore(10, 25);
            Assert.AreEqual(10, result.ElementAt(0).Score);
            Assert.AreEqual(15, result.ElementAt(1).Score);
            Assert.AreEqual(20, result.ElementAt(2).Score);
            Assert.AreEqual(25, result.ElementAt(3).Score);
        }
    }
}
