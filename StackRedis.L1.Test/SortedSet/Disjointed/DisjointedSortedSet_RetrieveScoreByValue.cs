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
    public class DisjointedSortedSet_RetrieveScoreByValue
    {
        [TestMethod]
        public void DisjointedSortedSet_RetrieveScoreByValue_Simple()
        {
            var set = new DisjointedSortedSet();
            set.Add(new[]
            {
                new StackExchange.Redis.SortedSetEntry("anna", 10),
                new StackExchange.Redis.SortedSetEntry("bob", 20)
            });
            Assert.AreEqual(10, set.RetrieveScoreByValue("anna"));
            Assert.IsNull(set.RetrieveScoreByValue("nonexistant"));
        }
    }
}
