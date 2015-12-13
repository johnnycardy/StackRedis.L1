using StackRedis.L1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.KeyspaceNotifications
{
    internal sealed class DatabaseRegister : IDisposable
    {
        internal static DatabaseRegister Instance = new DatabaseRegister();

        private Dictionary<string, DatabaseInstanceData> dbData = new Dictionary<string, DatabaseInstanceData>();
        private static readonly object _lockObj = new object();

        private DatabaseRegister()
        { }

        internal void RemoveInstanceData(IDatabase redisDb)
        {
            string dbIdentifier = string.Format("{0}:db={1}", redisDb.Multiplexer.Configuration, redisDb.Database);
            lock (_lockObj)
            {
                if (dbData.ContainsKey(dbIdentifier))
                {
                    dbData.Remove(dbIdentifier);
                }
            }
        }

        internal DatabaseInstanceData GetDatabaseInstanceData(IDatabase redisDb)
        {
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

        public void Dispose()
        {
            foreach(var db in dbData)
            {
                db.Value.Dispose();
            }

            dbData = new Dictionary<string, DatabaseInstanceData>();
        }
    }

    /// <summary>
    /// Holds details for each Redis database.
    /// </summary>
    internal class DatabaseInstanceData : IDisposable
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

        public void Dispose()
        {
            Listener.Dispose();
            MemoryCache.Dispose();
        }
    }
}
