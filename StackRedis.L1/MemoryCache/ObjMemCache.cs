using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache
{
    internal sealed class ObjMemCache : IDisposable
    {
        private static readonly object _lockObj = new object();

        //object storage
        private System.Runtime.Caching.MemoryCache _cache;
        
        //When you add an item to MemoryCache with a specific TTL, you can't retrieve it again.
        //So, we store them separately.
        private readonly ConcurrentDictionary<string, DateTimeOffset?> _ttls = new ConcurrentDictionary<string, DateTimeOffset?>();
        
        internal ObjMemCache()
        {
            Flush();
        }
        
        public bool ContainsKey(string key)
        {
            lock (_lockObj)
            {
                return _cache.Contains(key);
            }
        }

        public void Update(string key, object o)
        {
            lock (_lockObj)
            {
                if (!_cache.Contains(key)) return;
                var expiry = GetExpiryInternal(key);
                var expiryTimespan = expiry.HasValue ? expiry.Value : null;
                AddSingle(key, o, expiryTimespan, When.Always);
            }
        }

        private void AddSingle(string key, object o, TimeSpan? expiry, When when)
        {
            if (_cache == null) return;

            if (when == When.Exists && !_cache.Contains(key))
            {
                return;
            }

            if (when == When.NotExists && _cache.Contains(key))
            {
                return;
            }

            _cache.Remove(key);

            var policy = new CacheItemPolicy();

            if (expiry.HasValue && expiry.Value != default(TimeSpan))
            {
                //Store the ttl separately
                policy.AbsoluteExpiration = DateTime.UtcNow.Add(expiry.Value);
                _ttls[key] = policy.AbsoluteExpiration;
            }
            else
            {
                _ttls[key] = null;
            }

            System.Diagnostics.Debug.WriteLine("Adding key to mem cache: " + key);
            _cache.Set(key, o, policy);
        }

        public void Add(string key, object o, TimeSpan? expiry, When when)
        {
            lock (_lockObj)
            {
                AddSingle(key, o, expiry, when);
            }
        }

        public void Add(IEnumerable<KeyValuePair<RedisKey , RedisValue>> items, TimeSpan? expiry, When when)
        {
            lock (_lockObj)
            {
                foreach (var item in items)
                {
                    AddSingle(item.Key, item.Value, expiry, when);
                }
            }
        }

        public ValOrRefNullable<T> Get<T>(string key)
        {
            lock (_lockObj)
            {
                if (_cache.Contains(key))
                {
                    System.Diagnostics.Debug.WriteLine("Mem cache hit: " + key);
                    var result = (T)_cache[key];
                    return new ValOrRefNullable<T>(result);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Mem cache miss: " + key);
                }
            }

            return new ValOrRefNullable<T>();
        }

        /// <summary>
        /// Removes the stored TTL. Note that the key will still expire after the same timespan, it just won't be returned.
        /// To properly update the TTL, call Expire.
        /// </summary>
        internal void ClearTimeToLive(RedisKey key)
        {
            lock (_lockObj)
            {
                if (_ttls.ContainsKey(key))
                {
                    _ttls.TryRemove(key, out var t);
                }
            }
        }

        private ValOrRefNullable<TimeSpan?> GetExpiryInternal(string key)
        {
            if(_ttls.ContainsKey(key))
            {
                //There is a TTL stored. Is it null?
                if (_ttls[key].HasValue)
                {
                    //Return it as a timespan
                    return new ValOrRefNullable<TimeSpan?>(_ttls[key].Value.Subtract(DateTime.UtcNow));
                }
                else
                {
                    //There is a null TTL stored (ie, we know it's a non-expiring key)
                    return new ValOrRefNullable<TimeSpan?>(null);
                }
            }
            else
            {
                //There is no TTL stored - we would have to go to redis to get it.
                return new ValOrRefNullable<TimeSpan?>();
            }
        }

        public ValOrRefNullable<TimeSpan?> GetExpiry(string key)
        {
            lock (_lockObj)
            {
                return GetExpiryInternal(key);
            }
        }

        public bool Expire(string key, DateTimeOffset? expiry)
        {
            TimeSpan? diff = null;

            if(expiry.HasValue && expiry != default(DateTimeOffset))
            {
                diff = expiry.Value.Subtract(DateTime.UtcNow);
            }

            return Expire(key, diff);
        }

        public bool Expire(string key, TimeSpan? expiry)
        {
            var o = Get<object>(key);
            if(o.HasValue)
            {
                if (expiry == null || expiry.Value.TotalMilliseconds > 0)
                {
                    //Re-add with the given expiry
                    Add(key, o.Value, expiry, When.Always);
                }
                else
                {
                    Remove(new[] { key });
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RenameKey(string keyFrom, string keyTo)
        {
            lock (_lockObj)
            {
                if (_cache.Contains(keyFrom) && keyFrom != keyTo)
                {
                    System.Diagnostics.Debug.WriteLine("RenameKey: from " + keyFrom + " to " + keyTo);

                    var value = _cache.Get(keyFrom);
                    _cache.Remove(keyFrom);
                    
                    //Get the existing TTL
                    TimeSpan? ttl = null;
                    if(_ttls.ContainsKey(keyFrom))
                    {
                        if (_ttls[keyFrom].HasValue)
                        {
                            ttl = _ttls[keyFrom].Value.Subtract(DateTime.UtcNow);
                        }

                        //Remove the existing TTL
                        _ttls.TryRemove(keyFrom, out var t);
                    }
                    
                    Add(keyTo, value, ttl, When.Always);

                    return true;  
                }

                return false;
            }
        }

        public long Remove(string[] keys)
        {
            long result = 0;
            lock (_lockObj)
            {
                foreach (string key in keys)
                {
                    if (!string.IsNullOrEmpty(key) && _cache.Contains(key))
                    {
                        System.Diagnostics.Debug.WriteLine("Removing key from memcache: " + key);
                        _cache.Remove(key);
                        
                        if (_ttls.ContainsKey(key))
                        {
                            _ttls.TryRemove(key, out var t);
                        }

                        result++;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Flush()
        {
            lock (_lockObj)
            {
                if(_cache != null)
                {
                    _cache.Dispose();
                    _ttls.Clear();
                }

                _cache = new System.Runtime.Caching.MemoryCache("stackredis.l1.objmemcache");
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
