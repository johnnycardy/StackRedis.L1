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

        internal async Task<RedisValue> Get(string hashKey, RedisValue key, Func<RedisValue, Task<RedisValue>> retrieval)
        {
            return (await GetMulti(hashKey, new RedisValue[] { key }, async (keys) =>
            {
                RedisValue result = await retrieval(key);
                return new[] { result };
            })).Single();
        }

        internal async Task<RedisValue[]> GetMulti(string hashKey, RedisValue[] keys, Func<RedisValue[], Task<RedisValue[]>> retrieval)
        {
            if (!keys.Any())
                return new RedisValue[0];

            //Get the in-memory hash
            var hash = GetHash(hashKey);
            if (hash == null)
                hash = SetHash(hashKey);

            RedisValue[] result = new RedisValue[keys.Length];
            List<int> nonCachedIndices = new List<int>();

            for (int i = 0; i < keys.Length; i++)
            {
                if (hash.ContainsKey(keys[i]))
                {
                    result[i] = hash[keys[i]];
                }
                else
                {
                    nonCachedIndices.Add(i);
                }
            }

            //Get all non cached indices from redis and place them in their correct positions for the result array
            if (nonCachedIndices.Any())
            {
                RedisValue[] nonCachedKeys = keys.Where((key, index) => nonCachedIndices.Contains(index)).ToArray();
                RedisValue[] redisResults = await retrieval(nonCachedKeys);
                if (redisResults != null)
                {
                    int i = 0;
                    foreach (var redisResult in redisResults)
                    {
                        int originalIndex = nonCachedIndices[i++];
                        result[originalIndex] = redisResult;

                        //Cache this key for next time
                        hash.Add(keys[originalIndex], redisResult);
                    }
                }
            }

            return result;
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
        
        internal long Set(string hashKey, HashEntry[] hashEntries, When when = When.Always)
        {
            long result = 0;
            var hash = GetHash(hashKey);
            if (hash == null)
                hash = SetHash(hashKey);
            
            foreach (HashEntry entry in hashEntries)
            {
                if (when ==  When.Always || (hash.ContainsKey(entry.Name) && when == When.Exists))
                {
                    hash.Remove(entry.Name);

                    //Add the key
                    hash.Add(entry.Name, entry.Value);
                    result++;
                }
                else if(!hash.ContainsKey(entry.Name) && when == When.NotExists)
                {
                    //Add the key
                    hash.Add(entry.Name, entry.Value);
                    result++;
                }
            }

            return result;
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

        private Dictionary<string,RedisValue> SetHash(string hashKey)
        {
            var hash = new Dictionary<string, RedisValue>();
            _objMemCache.Add(hashKey, hash, null, When.Always);
            return hash;
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
