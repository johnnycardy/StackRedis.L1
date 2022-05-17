using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class StringLength : UnitTestBase
    {
        [TestMethod]
        public void StringLength_Simple()
        {
            _memDb.StringSet("key", "value", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            //length should be retrievable from cache
            Assert.AreEqual(5, (int)_memDb.StringLength("key"));
            Assert.AreEqual(1, CallsByMemDb);
        }
        
        [TestMethod]
        public async Task StringLength_StringChangedInRedis()
        {
            _memDb.StringSet("key", "value", null, When.Always);
            Assert.AreEqual(1, CallsByMemDb);

            //Get the length from the cache
            Assert.AreEqual(5, (int)_memDb.StringLength("key"));
            Assert.AreEqual(1, CallsByMemDb);

            await Task.Delay(50);

            //Change the string in redis
            _otherClientDb.StringSet("key", "longer value", null, When.Always);

            await Task.Delay(30); //Wait for the keyspace notification to remove the string

            //Length should be changed
            Assert.AreEqual(12, (int)_memDb.StringLength("key"));
            Assert.AreEqual(2, CallsByMemDb);
        }
    }
}
