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
    public class DisjointedSortedSetTest
    {
        [TestMethod]
        public void DisjointedSet_Add_OneEntry()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10) });
            var result = set.RetrieveByScore(10, 10, Exclude.None);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_OneEntry_ExcludeBoth()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10) });
            var result = set.RetrieveByScore(10, 10, Exclude.Both);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_Duplicate()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10) });
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 20) });
            Assert.IsNull(set.RetrieveByScore(10, 10, Exclude.None));
            Assert.AreEqual(1, set.RetrieveByScore(20, 20, Exclude.None).Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_Duplicate_ExcludeBoth()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10) });
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 20) });
            Assert.IsNull(set.RetrieveByScore(10, 10, Exclude.Both));
            Assert.AreEqual(0, set.RetrieveByScore(20, 20, Exclude.Both).Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_OneRange_TwoEntries_RetrieveByBoundaries()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10),
                            new StackExchange.Redis.SortedSetEntry("bob", 20)});
            var result = set.RetrieveByScore(10, 20, Exclude.None);
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
            var result = set.RetrieveByScore(19, 31, Exclude.None);
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
            var result = set.RetrieveByScore(5, 25, Exclude.None);
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
            var result = set.RetrieveByScore(25, 45, Exclude.None);
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
            var result = set.RetrieveByScore(5, 45, Exclude.None);
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
            var result = set.RetrieveByScore(15, 18, Exclude.None);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void DisjointedSet_Add_TwoRanges_RetrieveByBoundaries()
        {
            DisjointedSortedSet set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("anna", 10)});
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("bob", 20) });
            Assert.IsNull(set.RetrieveByScore(10, 20, Exclude.None));
            Assert.AreEqual(1, set.RetrieveByScore(10, 10, Exclude.None).Count());
            Assert.AreEqual(1, set.RetrieveByScore(20, 20, Exclude.None).Count());

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

            Assert.AreEqual(4, set.RetrieveByScore(10, 40, Exclude.None).Count());
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

            Assert.AreEqual(4, set.RetrieveByScore(10, 40, Exclude.None).Count());
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

            Assert.AreEqual(4, set.RetrieveByScore(10, 40, Exclude.None).Count());
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

            Assert.IsNull(set.RetrieveByScore(5, 50, Exclude.None));
            Assert.AreEqual(1, set.RetrieveByScore(10, 10, Exclude.None).Count());
            Assert.AreEqual(3, set.RetrieveByScore(15, 45, Exclude.None).Count());
            Assert.AreEqual(5, set.RetrieveByScore(10, 50, Exclude.None).Count());
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

            //One more to merge with the one above
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("carol", 25),
                new StackExchange.Redis.SortedSetEntry("sue", 35)
            });

            Assert.IsNull(set.RetrieveByScore(5, 50, Exclude.None));
            Assert.AreEqual(1, set.RetrieveByScore(20, 20, Exclude.None).Count());
            Assert.AreEqual(3, set.RetrieveByScore(25, 35, Exclude.None).Count());
            Assert.AreEqual(4, set.RetrieveByScore(25, 40, Exclude.None).Count());

            //A set to merge all three
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("emma", 5),
                new StackExchange.Redis.SortedSetEntry("frank", 50)
            });

            Assert.AreEqual(7, set.RetrieveByScore(5, 50, Exclude.None).Count());
        }

        [TestMethod]
        public void SortedSet_AddSingleItems_ThenRange()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("one", 1) });
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("two", 2) });
            set.Add(new[] { new StackExchange.Redis.SortedSetEntry("three", 3) });

            //Add a range that unites the above
            set.Add(new[] {
                new StackExchange.Redis.SortedSetEntry("zero", 0),
                new StackExchange.Redis.SortedSetEntry("one", 1),
                new StackExchange.Redis.SortedSetEntry("two", 2),
                new StackExchange.Redis.SortedSetEntry("three", 3),
                new StackExchange.Redis.SortedSetEntry("four", 4) });

            Assert.AreEqual(5, set.RetrieveByScore(0, 4, Exclude.None).Count());
            Assert.AreEqual(3, set.RetrieveByScore(0, 4, Exclude.Both).Count());
            Assert.AreEqual(4, set.RetrieveByScore(0, 4, Exclude.Start).Count());
            Assert.AreEqual(4, set.RetrieveByScore(0, 4, Exclude.Stop).Count());
        }
    }
}
