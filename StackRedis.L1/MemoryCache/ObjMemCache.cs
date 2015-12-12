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
        private System.Runtime.Caching.MemoryCache _cache;

        //When you add an item to MemoryCache with a specific TTL, you can't retrieve it again.
        //So, we store them separately.
        private Dictionary<string, DateTime> _ttls = new Dictionary<string, DateTime>();
        
        internal ObjMemCache()
        {
            Flush();
        }

        public bool ContainsKey(string key)
        {
            return _cache.Contains(key);
        }
        
        public void Add(string key, object o, TimeSpan? expiry, When when)
        {
            lock(_lockObj)
            {
                if (when == When.Exists && !_cache.Contains(key))
                {
                    return;
                }

                if (when == When.NotExists && _cache.Contains(key))
                {
                    return;
                }

                _cache.Remove(key);

                CacheItemPolicy policy = new CacheItemPolicy();

                if (expiry.HasValue && expiry.Value != default(TimeSpan))
                {
                    DateTime expiryDateTime = DateTime.UtcNow.Add(expiry.Value);

                    //Store the ttl separately
                    _ttls[key] = expiryDateTime;

                    policy.AbsoluteExpiration = new DateTimeOffset(expiryDateTime);
                }

                System.Diagnostics.Debug.WriteLine("Adding key to mem cache: " + key);
                _cache.Add(key, o, policy);
            }
        }

        public ValOrRefNullable<T> Get<T>(string key)
        {
            lock (_lockObj)
            {
                if (_cache.Contains(key))
                {
                    T result = (T)_cache[key];
                    return new ValOrRefNullable<T>(result);
                }
            }

            return new ValOrRefNullable<T>();
        }

        public void Expire(string key, DateTime? expiry)
        {
            TimeSpan? diff = null;

            if(expiry.HasValue && expiry != default(DateTime))
            {
                diff = expiry.Value.Subtract(DateTime.UtcNow);
            }

            Expire(key, diff);
        }

        public void Expire(string key, TimeSpan? expiry)
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
            }
        }

        public void RenameKey(string keyFrom, string keyTo)
        {
            lock (_lockObj)
            {
                if (_cache.Contains(keyFrom))
                {
                    System.Diagnostics.Debug.WriteLine("RenameKey: from " + keyFrom + " to " + keyTo);

                    var value = _cache.Get(keyFrom);
                    _cache.Remove(keyFrom);

                    //Get the existing TTL
                    TimeSpan? ttl = null;
                    if(_ttls.ContainsKey(keyFrom))
                    {
                        ttl = _ttls[keyFrom].Subtract(DateTime.UtcNow);
                    }

                    Add(keyTo, value, ttl, When.Always);   
                }
            }
        }

        public void Remove(string[] keys)
        {
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
                            _ttls.Remove(key);
                        }
                    }
                }
            }
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

                _cache = new System.Runtime.Caching.MemoryCache("cardy.redis.objmemcache");
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
