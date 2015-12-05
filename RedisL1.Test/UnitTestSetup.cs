using RedisL1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisL1.Test
{
    internal static class UnitTestSetup
    {
        internal static CallMonitoringRedisDatabase SetUp()
        {
            ObjMemCache.Instance.Flush();

            var options = new ConfigurationOptions();
            options.AllowAdmin = true;
            options.EndPoints.Add("localhost");

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);
            var database = connection.GetDatabase();
            var server = connection.GetServer(connection.GetEndPoints().First());
            server.FlushAllDatabases();

            return new CallMonitoringRedisDatabase(database);
        }

    }
}
