using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisL1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisL1.Test
{
    public class UnitTestBase
    {
        protected CallMonitoringRedisDatabase _redisDb;
        protected RedisL1Database _memDb;

        [TestInitialize]
        public void SetUp()
        {
            var options = new ConfigurationOptions();
            options.AllowAdmin = true;
            options.EndPoints.Add("localhost");

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);
            var server = connection.GetServer(connection.GetEndPoints().First());
            var database = connection.GetDatabase();

            //Implemention of IDatabase that counts calls (so tests can tell which requests made it to the network)
            _redisDb = new CallMonitoringRedisDatabase(database);

            //Construct the in-memory cache
            _memDb = new RedisL1Database(_redisDb);

            //Clean everything out
            server.FlushAllDatabases();
            _memDb.Flush();

            //Reset the number of calls
            _redisDb.Calls = 0;
            
            //Pause keyspace notifications during most tests
            _memDb.DBData.Listener.Paused = true;
        }

    }
}
