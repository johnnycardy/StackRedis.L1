using StackRedis.L1.MemoryCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.KeyspaceNotifications
{
    internal class NotificationListener : IDisposable
    {
        private static readonly string _keyspace = "__keyspace@0__:";
        private List<DatabaseInstanceData> _databases = new List<DatabaseInstanceData>();
        private ISubscriber _subscriber;

        private FixedSizedQueue<KeyValuePair<string, string>> _notificationHistory = new FixedSizedQueue<KeyValuePair<string, string>>(5);
        
        internal bool Paused { get; set; }

        internal NotificationListener(ConnectionMultiplexer connection)
        {
            _subscriber = connection.GetSubscriber();
            _subscriber.Subscribe(_keyspace + "*", (channel, value) =>
            {
                if (!Paused)
                {
                    string key = ((string)channel).Replace(_keyspace, "");
                    HandleKeyspaceEvent(new KeyValuePair<string, string>(key, value));
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

        private void HandleKeyspaceEvent(KeyValuePair<string, string> kvp)
        {
            foreach(DatabaseInstanceData dbData in _databases)
            {
                HandleKeyspaceEvent(dbData, kvp);
            }

            //Store the event
            _notificationHistory.Enqueue(kvp);
        }

        /// <summary>
        /// Reads the key/value and updates the database with the relevant value
        /// </summary>
        private void HandleKeyspaceEvent(DatabaseInstanceData dbData, KeyValuePair<string,string> kvp)
        {
            System.Diagnostics.Debug.WriteLine("Keyspace event. Key=" + kvp.Key + ", Value=" + kvp.Value);

            //Handle DEL, RENAME and EXPIRE
            if(kvp.Value == "del")
            {
                dbData.MemoryCache.Remove(new[] { kvp.Key });
            }
            else if(kvp.Value == "rename_to")
            {
                //the previous event should be "rename_from" and contains the current key
                if(_notificationHistory.Any() && _notificationHistory.Last().Value == "rename_from")
                {
                    dbData.MemoryCache.RenameKey(_notificationHistory.Last().Key, kvp.Key);
                }
            }
            else if(kvp.Value == "expired")
            {
                dbData.MemoryCache.Remove(new[] { kvp.Key });
                System.Diagnostics.Debug.WriteLine("Key expired and removed:" + kvp.Key);
            }
        }
    }
}
