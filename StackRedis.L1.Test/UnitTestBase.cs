using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackRedis.L1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StackRedis.L1.Test
{
    public class UnitTestBase
    {
        protected IDatabase _redisDirectDb;
        protected NotificationDatabase _otherClientDb;
        protected RedisL1Database _memDb;

        private CallMonitoringRedisDatabase _callMonMemDb;

        protected int CallsByMemDb
        {
            get { return _callMonMemDb.Calls; }
        }

        [TestInitialize]
        public void SetUp()
        {
            var options = new ConfigurationOptions();
            options.AllowAdmin = true;
            options.EndPoints.Add("localhost");

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);
            var server = connection.GetServer(connection.GetEndPoints().First());
            server.ConfigSet("notify-keyspace-events", "KEA");

            _redisDirectDb = connection.GetDatabase();
            
            //Construct the in-memory cache using an Implemention of IDatabase that counts calls (so tests can tell which requests made it to the network)
            _callMonMemDb = new CallMonitoringRedisDatabase(_redisDirectDb);
            _memDb = new RedisL1Database(_callMonMemDb);

            //Get a notification database to simulate another client
            _otherClientDb = new NotificationDatabase(_redisDirectDb, "other client");
            
            //Clean everything out
            server.FlushAllDatabases();
            _memDb.Flush();

            _callMonMemDb.Calls = 0;
        }

        [TestCleanup]
        public void TearDown()
        {
            _memDb.Dispose();
        }

    }
}
