using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            return 0;
        }

        internal RedisValueWithExpiry CreateRedisValueWithExpiry(RedisValue value, TimeSpan? expiry)
        {
            var result = new RedisValueWithExpiry();
            
            //Box into object so that we can set properties on the same instance
            object oResult = result;

            result.GetType().GetField("expiry", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(oResult, expiry);
            result.GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(oResult, value);

            //Unbox back to struct
            result = (RedisValueWithExpiry)oResult;
            
            return result;

        }

        internal async Task<RedisValueWithExpiry> GetFromMemoryWithExpiry(string key, Func<Task<RedisValueWithExpiry>> retrieval)
        {
            ValOrRefNullable<RedisValue> cachedValue = _memCache.Get<RedisValue>(key);
            if (cachedValue.HasValue)
            {
                //If we know the expiry, then a trip to redis isn't necessary.
                var expiry = _memCache.GetExpiry(key);
                if (expiry.HasValue)
                {
                    return CreateRedisValueWithExpiry(cachedValue.Value, expiry.Value);
                }
            }
            
            RedisValueWithExpiry result = await retrieval();

            //Cache the value and expiry
            _memCache.Add(key, result.Value, result.Expiry, When.Always);

            return result;
        }
        
        internal async Task<RedisValue> GetFromMemory(string key, Func<Task<RedisValue>> retrieval)
        {
            return (await GetFromMemoryMulti(new RedisKey[] { key }, async (keys) =>
            {
                RedisValue result = await retrieval();
                return new [] { result };
            })).Single();
        }
        
        internal async Task<RedisValue[]> GetFromMemoryMulti(RedisKey[] keys, Func<RedisKey[], Task<RedisValue[]>> retrieval)
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

        internal bool SetStringBit(RedisKey key, long offset, bool bit)
        {
            var existingString = _memCache.Get<RedisValue>(key);
            if(existingString.HasValue)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(existingString.Value);
                BitArray bitArray = new BitArray(bytes);
                if(bitArray.Length > offset)
                {
                    bool existingValue = bitArray[(int)offset];

                    bitArray[(int)offset] = bit;

                    //Copy back to byte array
                    bitArray.CopyTo(bytes, 0);

                    //Re-save as a string
                    string str = Encoding.UTF8.GetString(bytes);
                    _memCache.Update(key, str);

                    //Return the existing value
                    return existingValue;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Create an empty string
                _memCache.Add(key, "", null, When.NotExists);
                
                //The existing value is 0
                return false;
            }
        }
    }
}
