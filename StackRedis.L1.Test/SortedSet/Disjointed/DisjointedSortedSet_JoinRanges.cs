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
    public class DisjointedSortedSet_JoinRanges
    {
        [TestMethod]
        public void DisjointedSortedSet_JoinRanges_AlreadyJoined()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2),
                new StackExchange.Redis.SortedSetEntry("three", 3),
                new StackExchange.Redis.SortedSetEntry("four", 4) });

            set.JoinRanges(new RedisValue[] { "one", "two" });

            Assert.AreEqual(5, set.RetrieveByScore(0, 4, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSortedSet_JoinRanges_NoContinuousValues()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2),
                new StackExchange.Redis.SortedSetEntry("three", 3),
                new StackExchange.Redis.SortedSetEntry("four", 4) });

            set.JoinRanges(new RedisValue[] { "one", "three" });

            Assert.AreEqual(5, set.RetrieveByScore(0, 4, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSortedSet_JoinRanges_OutsideExistingRange()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2),
                new StackExchange.Redis.SortedSetEntry("three", 3),
                new StackExchange.Redis.SortedSetEntry("four", 4) });

            set.JoinRanges(new RedisValue[] { "four", "five" });

            Assert.AreEqual(5, set.RetrieveByScore(0, 4, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSortedSet_JoinRanges_Joins_RangesFullyMatches()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1) });
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("two", 2),
                new StackExchange.Redis.SortedSetEntry("three", 3) });
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("four", 4),
                new StackExchange.Redis.SortedSetEntry("five", 5),
                new StackExchange.Redis.SortedSetEntry("six", 6) });

            Assert.IsNull(set.RetrieveByScore(2, 6, Exclude.None));

            set.JoinRanges(new RedisValue[] { "three", "four", "five", "six" });

            Assert.AreEqual(5, set.RetrieveByScore(2, 6, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSortedSet_JoinRanges_Joins_RangesPartiallyMatches()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1) });
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("two", 2),
                new StackExchange.Redis.SortedSetEntry("three", 3) });
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("four", 4),
                new StackExchange.Redis.SortedSetEntry("five", 5) });

            Assert.IsNull(set.RetrieveByScore(2, 5, Exclude.None));

            set.JoinRanges(new RedisValue[] { "three", "four", "five", "six" });

            Assert.AreEqual(4, set.RetrieveByScore(2, 5, Exclude.None).Count());
        }
    }
}
