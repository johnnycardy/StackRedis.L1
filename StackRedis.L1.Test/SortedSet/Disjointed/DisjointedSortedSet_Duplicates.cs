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
    public class DisjointedSortedSet_Duplicates
    {

        [TestMethod]
        public void DisjointedSet_DuplicateScores()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 10)
            });

            Assert.AreEqual(2, set.RetrieveByScore(10, 10, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSet_DuplicateValues()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("anna", 10)
            });

            Assert.AreEqual(1, set.RetrieveByScore(10, 10, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSet_DuplicateValues_AddedSeparately()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10)
            });
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10)
            });

            Assert.AreEqual(1, set.RetrieveByScore(10, 10, Exclude.None).Count());
        }
    }
}
