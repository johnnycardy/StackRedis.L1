using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using RedisL1.MemoryCache;

namespace RedisL1.Test
{
    [TestClass]
    public class StringLength
    {
        [TestInitialize]
        public void SetUp()
        {
            ObjMemCache.Instance.Flush();
        }

        [TestMethod]
        public void StringLength_Simple()
        {
        }
        
    }
}
