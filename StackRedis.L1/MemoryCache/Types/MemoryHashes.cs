using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache.Types
{
    internal class MemoryHashes
    {
        private ObjMemCache _objMemCache;
        internal MemoryHashes(ObjMemCache objMemCache)
        {
            _objMemCache = objMemCache;
        }


        internal bool Contains(string hashKey, string key)
        {
            var hash = GetHash(hashKey);
            if (hash != null)
                return hash.ContainsKey(key);
            else
                return false;
        }

        internal RedisValue Get(string hashKey, string key)
        {
            var hash = GetHash(hashKey);
            if (hash != null && hash.ContainsKey(key))
            {
                return hash[key];
            }

            return new RedisValue();
        }
        
        internal void Set(string hashKey, string key, RedisValue value)
        {
            var hash = GetHash(hashKey);
            if (hash == null)
                hash = SetHash(hashKey, null, When.Always);

            //Remove it if it already is there
            if (hash.ContainsKey(key))
                hash.Remove(key);

            //Add the key
            hash.Add(key, value);
        }

        internal long Delete(string hashKey, RedisValue[] keys)
        {
            long result = 0;
            var hash = GetHash(hashKey);
            if (hash != null)
            {
                foreach (RedisValue key in keys)
                {
                    if (hash.Remove(key))
                        result++;
                }
            }
            return result;
        }

        private Dictionary<string,RedisValue> SetHash(string hashKey, TimeSpan? timeout, When when)
        {
            _objMemCache.Add(hashKey, new Dictionary<string, RedisValue>(), timeout, when);
            return _objMemCache.Get<Dictionary<string, RedisValue>>(hashKey).Value;
        }

        private Dictionary<string,RedisValue> GetHash(string hashKey)
        {
            var result = _objMemCache.Get<Dictionary<string, RedisValue>>(hashKey);
            if(result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
