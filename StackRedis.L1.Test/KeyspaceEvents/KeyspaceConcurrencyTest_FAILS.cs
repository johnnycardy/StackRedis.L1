using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test
{
    [TestClass]
    public class KeyspaceConcurrencyTestFAILS : UnitTestBase
    {
        /// <summary>
        /// This is the test to illustrate the 'recent keys' problem.
        /// 
        /// If two values set are from different servers within a very close timespan (see ObjMemCache.RecentKeyMilliseconds)
        /// There are two scenarios:
        /// 1 - the notification is from this server, not another. We have the correct value in memory and shouldn't delete it.
        /// 2 - the notification is from another server. We have the incorrect value in memory and should delete it.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task KeyspaceConcurrencyTest_FAILS()
        {
            //Set a string in memory
            _memDb.StringSet("key", "value1");

            //Wait a length of time less than ObjMemCache.RecentKeyMilliseconds
            await Task.Delay(ObjMemCache.RecentKeyMilliseconds / 2); 

            //Set the same key redis (not using the memory database)
            _redisDb.StringSet("key", "value2");
            
            //Wait 100ms for notifications to be delivered
            await Task.Delay(100);

            //Value should be updated but isn't.
            Assert.AreEqual("value2", (string)_memDb.StringGet("key"));
        }

        [TestMethod]
        public async Task KeyspaceConcurrencyTest_PASSES()
        {
            //Set a string in memory
            _memDb.StringSet("key", "value1");

            //Wait a length of time greater than ObjMemCache.RecentKeyMilliseconds
            await Task.Delay(ObjMemCache.RecentKeyMilliseconds + 100);

            //Set the same key redis (not using the memory database)
            _redisDb.StringSet("key", "value2");

            //Wait 100ms for notifications to be delivered
            await Task.Delay(100);

            //Value should be updated.
            Assert.AreEqual("value2", (string)_memDb.StringGet("key"));
        }
    }
}
