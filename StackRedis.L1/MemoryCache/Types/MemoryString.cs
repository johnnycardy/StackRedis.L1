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
        private ObjMemCache _memCache;
        
        internal MemoryStrings(ObjMemCache memCache)
        {
            _memCache = memCache;
        }

        internal long GetStringLength(string key)
        {
            if(_memCache.ContainsKey(key))
            {
                var value = _memCache.Get<RedisValue>(key);
                if(value.HasValue)
                {
                    RedisValue redisValue = value.Value;
                    if(redisValue.HasValue)
                    {
                        return ((string)redisValue).Length;
                    }
                }
            }

            return -1;
        }
        
        internal async Task<RedisValue> MultiValueGetFromMemory(string key, Func<Task<RedisValue>> retrieval)
        {
            return (await MultiValueGetFromMemory(new RedisKey[] { key }, async (keys) =>
            {
                RedisValue result = await retrieval();
                return new [] { result };
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
                    foreach (var redisResult in redisResults)
                    {
                        int originalIndex = nonCachedIndices[i++];
                        result[originalIndex] = redisResult;

                        //Cache this key for next time
                        _memCache.Add(keys[originalIndex], redisResult, null, When.Always);
                    }
                }
            }

            return result;
        }
    }
}
