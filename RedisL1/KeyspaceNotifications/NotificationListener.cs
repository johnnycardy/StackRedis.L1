using RedisL1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisL1.KeyspaceNotifications
{
    internal class NotificationListener : IDisposable
    {
        private static readonly string _keyspace = "__keyspace@0__:";
        private List<DatabaseInstanceData> _databases = new List<DatabaseInstanceData>();
        private ISubscriber _subscriber;

        internal bool Paused { get; set; }

        internal NotificationListener(ConnectionMultiplexer connection)
        {
            _subscriber = connection.GetSubscriber();
            _subscriber.Subscribe(_keyspace + "*", (channel, value) =>
            {
                if (!Paused)
                {
                    string key = ((string)channel).Replace(_keyspace, "");
                    HandleKeyspaceEvent(key, value);
                }
            });
        }
        
        public void Dispose()
        {
            _subscriber.Unsubscribe(_keyspace);
        }

        internal void HandleKeyspaceEvents(DatabaseInstanceData dbData)
        {
            _databases.Add(dbData);
        }

        private void HandleKeyspaceEvent(string key, string value)
        {
            foreach(DatabaseInstanceData dbData in _databases)
            {
                HandleKeyspaceEvent(dbData, key, value);
            }
        }

        /// <summary>
        /// Reads the key/value and updates the database with the relevant value
        /// </summary>
        private void HandleKeyspaceEvent(DatabaseInstanceData dbData, string key, string value)
        {
            //Handle DEL, RENAME and EXPIRE
            if(value == "del")
            {
                dbData.MemoryCache.Remove(new[] { key });
            }
        }
    }
}
