using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace RedisL1.MemoryCache
{
    internal sealed class ObjMemCache : IDisposable
    {
        private static readonly object _lockObj = new object();
        private System.Runtime.Caching.MemoryCache _cache;

        internal static ObjMemCache Instance = new ObjMemCache();

        internal ObjMemCache()
        {
            Create();
        }

        private void Create()
        {
            _cache = new System.Runtime.Caching.MemoryCache("cardy.redis.objmemcache");
        }
        
        public void Add(string key, object o, TimeSpan? expiry, When when)
        {
            if(when == When.Exists && !_cache.Contains(key))
            {
                return;
            }

            if(when == When.NotExists && _cache.Contains(key))
            {
                return;
            }

            _cache.Remove(key);

            CacheItemPolicy policy = new CacheItemPolicy();

            if(expiry.HasValue && expiry.Value != default(TimeSpan))
            {
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.Add(expiry.Value));
            }

            _cache.Add(key, o, policy);
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
        
        public void Remove(string[] keys)
        {
            lock (_lockObj)
            {
                foreach (string key in keys)
                {
                    if (!string.IsNullOrEmpty(key) && _cache.Contains(key))
                    {
                        _cache.Remove(key);
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
                _cache.Dispose();
                Create();
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
