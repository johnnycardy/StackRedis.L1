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
    public class DisjointedSortedSet_RemoveByScore
    {
        [TestMethod]
        public void DisjointedSortedSet_RemoveByScore_ExcludeNone()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2) });

            Assert.AreEqual(1, set.RetrieveByScore(0, 0, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(1, 1, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(2, 2, Exclude.None).Count());
            set.RemoveByScore(0, 2, Exclude.None);
            Assert.IsNull(set.RetrieveByScore(0, 0, Exclude.None));
            Assert.IsNull(set.RetrieveByScore(1, 1, Exclude.None));
            Assert.IsNull(set.RetrieveByScore(2, 2, Exclude.None));
        }

        [TestMethod]
        public void DisjointedSortedSet_RemoveByScore_ExcludeBoth()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2) });

            Assert.AreEqual(1, set.RetrieveByScore(0, 0, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(1, 1, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(2, 2, Exclude.None).Count());
            set.RemoveByScore(0, 2, Exclude.Both);
            Assert.AreEqual(1, set.RetrieveByScore(0, 0, Exclude.None).Count());
            Assert.IsNotNull(set.RetrieveByScore(1, 1, Exclude.None)); //There is still a valid range of "0, 2", which no longer contains 1. So we know 1 doesn't exist.
            Assert.AreEqual(0, set.RetrieveByScore(1, 1, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(2, 2, Exclude.None).Count());
        }


        [TestMethod]
        public void DisjointedSortedSet_RemoveByScore_ExcludeStart()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2) });

            Assert.AreEqual(1, set.RetrieveByScore(0, 0, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(1, 1, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(2, 2, Exclude.None).Count());
            set.RemoveByScore(0, 2, Exclude.Start);
            Assert.AreEqual(1, set.RetrieveByScore(0, 0, Exclude.None).Count());
            Assert.IsNull(set.RetrieveByScore(1, 1, Exclude.None));
            Assert.IsNull(set.RetrieveByScore(2, 2, Exclude.None));
        }

        [TestMethod]
        public void DisjointedSortedSet_RemoveByScore_ExcludeStop()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2) });

            Assert.AreEqual(1, set.RetrieveByScore(0, 0, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(1, 1, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(2, 2, Exclude.None).Count());
            set.RemoveByScore(0, 2, Exclude.Stop);
            Assert.IsNull(set.RetrieveByScore(0, 0, Exclude.None));
            Assert.IsNull(set.RetrieveByScore(1, 1, Exclude.None));
            Assert.AreEqual(1, set.RetrieveByScore(2, 2, Exclude.None).Count());
        }
    }
}
