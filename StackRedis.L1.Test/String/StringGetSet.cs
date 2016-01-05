using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.String
{
    [TestClass]
    public class StringGetSet : UnitTestBase
    {
        [TestMethod]
        public void StringGetSet_KeyPresent()
        {
            //Set up a string
            _redisDirectDb.StringSet("key1", "value1");
            Assert.AreEqual(0, CallsByMemDb);
            
            //StringGetSet always goes to redis to do the set part
            RedisValue result = _memDb.StringGetSet("key1", "value2");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual("value1", (string)result);

            //Should not need to go to redis this time
            result = _memDb.StringGet("key1");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual("value2", (string)result);
        }

        [TestMethod]
        public void StringGetSet_KeyNotPresent()
        {
            //StringGetSet always goes to redis to do the set part
            RedisValue result = _memDb.StringGetSet("key1", "value2");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.IsFalse(result.HasValue);

            //Should not need to go to redis this time
            result = _memDb.StringGet("key1");
            Assert.AreEqual(1, CallsByMemDb);
            Assert.AreEqual("value2", (string)result);
        }
    }
}
