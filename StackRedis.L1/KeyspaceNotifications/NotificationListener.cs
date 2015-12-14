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

        internal NotificationListener(ConnectionMultiplexer connection, Func<string, bool> recentKeyCheck)
        {
            _subscriber = connection.GetSubscriber();
            _subscriber.Subscribe(_keyspace + "*", (channel, value) =>
            {
                if (!Paused)
                {
                    string key = ((string)channel).Replace(_keyspace, "");

                    HandleKeyspaceEvent(new KeyValuePair<string, string>(key, value), recentKeyCheck(key));
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

        private void HandleKeyspaceEvent(KeyValuePair<string, string> kvp, bool isRecentlyAdded)
        {
            foreach(DatabaseInstanceData dbData in _databases)
            {
                HandleKeyspaceEvent(dbData, kvp, isRecentlyAdded);
            }

            //Store the event
            _notificationHistory.Enqueue(kvp);
        }

        /// <summary>
        /// Reads the key/value and updates the database with the relevant value
        /// </summary>
        private void HandleKeyspaceEvent(DatabaseInstanceData dbData, KeyValuePair<string,string> kvp, bool isRecentlyAddedOnThisServer)
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
            else if(kvp.Value == "set")
            {
                if (isRecentlyAddedOnThisServer)
                {
                    //todo: now there has been a value set on another server and this server, within a very close timespan.
                    //1 - the notification may actually be from this server - not another. We don't know.
                    //2 - if a value is set on another server, we may or may not have an out of date value in memory. Again, we don't know.
                }
                else
                {
                    //A key has been set. If it exists in memory, it is probably now outdated.
                    dbData.MemoryCache.Remove(new[] { kvp.Key });
                }
            }
        }
    }
}
