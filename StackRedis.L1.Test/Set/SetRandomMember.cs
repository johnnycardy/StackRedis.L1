using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackRedis.L1.MemoryCache;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.Set
{
    [TestClass]
    public class SetRandomMember : UnitTestBase
    {
        [TestMethod]
        public void SetRandomMember_Simple()
        {
            //Add an item in redis
            _redisDirectDb.SetAdd("set", "value1");
            Assert.AreEqual("value1", (string)_memDb.SetRandomMember("set"));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(_memDb.SetContains("set", "value1"));
            Assert.AreEqual(1, CallsByMemDb); //Should be cached
        }

        [TestMethod]
        public void SetRandomMembers_Simple()
        {
            //Add an item in redis
            _redisDirectDb.SetAdd("set", "value1");
            Assert.AreEqual("value1", (string)_memDb.SetRandomMembers("set", 1)[0]);
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(_memDb.SetContains("set", "value1"));
            Assert.AreEqual(1, CallsByMemDb); //Should be cached
        }

        [TestMethod]
        public async Task SetRandomMemberAsync_Simple()
        {
            //Add an item in redis
            await _redisDirectDb.SetAddAsync("set", "value1");
            Assert.AreEqual("value1", (string)(await _memDb.SetRandomMemberAsync("set")));
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(await _memDb.SetContainsAsync("set", "value1"));
            Assert.AreEqual(1, CallsByMemDb); //Should be cached
        }

        [TestMethod]
        public async Task SetRandomMembersAsync_Simple()
        {
            //Add an item in redis
            await _redisDirectDb.SetAddAsync("set", "value1");
            Assert.AreEqual("value1", (string)(await _memDb.SetRandomMembersAsync("set", 1))[0]);
            Assert.AreEqual(1, CallsByMemDb);

            Assert.IsTrue(await _memDb.SetContainsAsync("set", "value1"));
            Assert.AreEqual(1, CallsByMemDb); //Should be cached
        }
    }
}