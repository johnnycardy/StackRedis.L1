using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache.Types
{
    internal class MemoryStrings
    {
        private TimeSpan? _defaultExpiry;
        private ObjMemCache _memCache;
        
        internal MemoryStrings(TimeSpan? defaultExpiry, ObjMemCache memCache)
        {
            _defaultExpiry = defaultExpiry;
            _memCache = memCache;
        }

        internal async Task<long> StringLengthAsync(string key, Func<Task<long>> retrieval)
        {
            //Check if the string is in memory
            var val = _memCache.Get<RedisValue>(key);
            if(val.HasValue)
            {
                return ((string)val.Value).Length;
            }
            else
            {
                //Check if we've already cached the length
                string lenKey = key + ":StackRedis.L1:StringLength";
                val = _memCache.Get<RedisValue>(lenKey);
                if(val.HasValue)
                {
                    return (long)val.Value;
                }
                else
                {
                    long length = await retrieval();
                    _memCache.Add(lenKey, length, _defaultExpiry, When.Always);
                    return length;
                }
            }
        }

        internal async Task<RedisValue> MultiValueGetFromMemory(string key, Func<Task<RedisValue>> retrieval)
        {
            return (await MultiValueGetFromMemory(new RedisKey[] { key }, async (keys) =>
            {
                RedisValue result = await retrieval();
                return new RedisValue[] { result };
            })).Single();
        }
        
        internal async Task<RedisValue[]> MultiValueGetFromMemory(RedisKey[] keys, Func<RedisKey[], Task<RedisValue[]>> retrieval)
        {
            if (!keys.Any())
                return new RedisValue[0];

            RedisValue[] result = new RedisValue[keys.Length];
            List<int> nonCachedIndices = new List<int>();

            for (int i = 0; i < keys.Length; i++)
            {
                var cachedItem = _memCache.Get<RedisValue>(keys[i]);
                if (cachedItem.HasValue)
                {
                    result[i] = cachedItem.Value;
                }
                else
                {
                    nonCachedIndices.Add(i);
                }
            }

            //Get all non cached indices from redis and place them in their correct positions for the result array
            if (nonCachedIndices.Any())
            {
                RedisKey[] nonCachedKeys = keys.Where((key, index) => nonCachedIndices.Contains(index)).ToArray();
                RedisValue[] redisResults = await retrieval(nonCachedKeys);
                if (redisResults != null)
                {
                    int i = 0;
                    foreach (RedisValue redisResult in redisResults)
                    {
                        int originalIndex = nonCachedIndices[i++];
                        result[originalIndex] = redisResult;

                        //Cache this key for next time
                        _memCache.Add(keys[originalIndex], redisResult, _defaultExpiry, When.Always);
                    }
                }
            }

            return result;
        }
    }
}
