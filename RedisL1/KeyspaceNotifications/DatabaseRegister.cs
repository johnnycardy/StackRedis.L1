using RedisL1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisL1.KeyspaceNotifications
{
    internal sealed class DatabaseRegister
    {
        internal static DatabaseRegister Instance = new DatabaseRegister();

        private Dictionary<string, DatabaseInstanceData> dbData = new Dictionary<string, DatabaseInstanceData>();
        private static readonly object _lockObj = new object();

        private DatabaseRegister()
        { }

        internal DatabaseInstanceData GetDatabaseInstanceData(IDatabase redisDb)
        {
            //create a unique string for this db
            var endpoint = redisDb.IdentifyEndpoint();
            
            string dbIdentifier = string.Format("{0}:db={1}", redisDb.Multiplexer.Configuration, redisDb.Database);

            //Check if this db is already registered, and register it for notifications if necessary
            lock (_lockObj)
            {
                if (!dbData.ContainsKey(dbIdentifier))
                {
                    dbData.Add(dbIdentifier, new DatabaseInstanceData(redisDb));
                }
            }

            return dbData[dbIdentifier];
        }
    }

    /// <summary>
    /// Holds details for each Redis database.
    /// </summary>
    internal class DatabaseInstanceData
    {
        /// <summary>
        /// The notification listener to handle keyspace notifications
        /// </summary>
        public NotificationListener Listener { get; private set; }

        /// <summary>
        /// Memory cache for this database
        /// </summary>
        public ObjMemCache MemoryCache { get; private set; }

        internal DatabaseInstanceData(IDatabase redisDb)
        {
            Listener = new NotificationListener(redisDb.Multiplexer);
            MemoryCache = new ObjMemCache();

            //Connect the memory cache to the listener. Its data will be updated when keyspace events occur.
            Listener.HandleKeyspaceEvents(this);
        }
    }
}
