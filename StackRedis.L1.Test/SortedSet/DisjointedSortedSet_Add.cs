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
    public class DisjointedSortedSetTest
    {
        [TestMethod]
        public void DisjointedSet_Add_OneEntry()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10) });
            var result = set.RetrieveByScore(10, 10);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_OneRange_TwoEntries_RetrieveByBoundaries()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20)});
            var result = set.RetrieveByScore(10, 20);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_OneRange_ManyEntries_RetrieveInsideBoundaries()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20),
                            new StackExchange.Redis.SortedSetEntry("carol", 30),
                            new StackExchange.Redis.SortedSetEntry("derek", 40)});
            var result = set.RetrieveByScore(19, 31);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("bob", (string)result.ElementAt(0).Element);
            Assert.AreEqual("carol", (string)result.ElementAt(1).Element);
        }


        [TestMethod]
        public void DisjointedSet_Add_OneRange_ManyEntries_RetrieveOutsideLowerBoundary()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20),
                            new StackExchange.Redis.SortedSetEntry("carol", 30),
                            new StackExchange.Redis.SortedSetEntry("derek", 40)});
            var result = set.RetrieveByScore(5, 25);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DisjointedSet_Add_OneRange_ManyEntries_RetrieveOutsideUpperBoundary()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20),
                            new StackExchange.Redis.SortedSetEntry("carol", 30),
                            new StackExchange.Redis.SortedSetEntry("derek", 40)});
            var result = set.RetrieveByScore(25, 45);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DisjointedSet_Add_OneRange_ManyEntries_RetrieveOutsideBothBoundaries()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20),
                            new StackExchange.Redis.SortedSetEntry("carol", 30),
                            new StackExchange.Redis.SortedSetEntry("derek", 40)});
            var result = set.RetrieveByScore(5, 45);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DisjointedSet_Add_OneRange_ManyEntries_RetrieveEmptyResult()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20),
                            new StackExchange.Redis.SortedSetEntry("carol", 30),
                            new StackExchange.Redis.SortedSetEntry("derek", 40)});
            var result = set.RetrieveByScore(15, 18);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_TwoRanges_Overlapped_Lower_Merged()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("bob", 20),
                new StackExchange.Redis.SortedSetEntry("derek", 40)
            });

            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("carol", 30)
            });

            Assert.AreEqual(4, set.RetrieveByScore(10, 40).Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_TwoRanges_Overlapped_Upper_Merged()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("carol", 30)
            });
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("bob", 20),
                new StackExchange.Redis.SortedSetEntry("derek", 40)
            });

            Assert.AreEqual(4, set.RetrieveByScore(10, 40).Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_TwoRanges_Overlapped_Inner_Merged()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();

            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("carol", 40)
            });
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("bob", 20),
                new StackExchange.Redis.SortedSetEntry("derek", 30)
            });

            Assert.AreEqual(4, set.RetrieveByScore(10, 40).Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_ThreeRanges_Overlapped_Outside_Merged()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();

            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 20)
            });
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("bob", 30),
                new StackExchange.Redis.SortedSetEntry("derek", 40)
            });

            //One more to merge into the two above
            set.Add(new[] 
            {
                new StackExchange.Redis.SortedSetEntry("carol", 10),
                new StackExchange.Redis.SortedSetEntry("sue", 50)
            });

            Assert.IsNull(set.RetrieveByScore(5, 50));
            Assert.AreEqual(1, set.RetrieveByScore(10, 10).Count());
            Assert.AreEqual(3, set.RetrieveByScore(15, 45).Count());
            Assert.AreEqual(5, set.RetrieveByScore(10, 50).Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_ThreeRanges_Overlapped_Inside_Merged()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();

            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 20)
            });
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("bob", 30),
                new StackExchange.Redis.SortedSetEntry("derek", 40)
            });

            //One more to merge into the two above
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("carol", 25),
                new StackExchange.Redis.SortedSetEntry("sue", 35)
            });

            Assert.IsNull(set.RetrieveByScore(5, 50));
            Assert.AreEqual(1, set.RetrieveByScore(20, 20).Count());
            Assert.AreEqual(3, set.RetrieveByScore(25, 35).Count());
            Assert.AreEqual(5, set.RetrieveByScore(20, 40).Count());
        }
    }
}
