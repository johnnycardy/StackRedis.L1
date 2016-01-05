using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using StackRedis.L1.MemoryCache;
using StackRedis.L1.MemoryCache.Types;

namespace StackRedis.L1.Test
{
    public class CallMonitoringRedisDatabase : RedisForwardingDatabase
    {
        public int Calls { get; set; }
        
        public CallMonitoringRedisDatabase(IDatabase redisDb)
            : base(redisDb)
        {
            _redisDb = redisDb;
            _onCall = () =>
            {
                Calls++;
            };
        }
    }
}
