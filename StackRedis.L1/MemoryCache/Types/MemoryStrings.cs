﻿using StackExchange.Redis;
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
        private readonly ObjMemCache _memCache;
        
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

            result.GetType().GetField("expiry", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(oResult, expiry);
            result.GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(oResult, value);

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

        internal RedisValue GetFromMemory(string key, Func<RedisValue> retrieval)
        {
            return GetFromMemoryMulti(new RedisKey[] { key }, keys =>
            {
                var result = retrieval();
                return new[] { result };
            }).Single();
        }

        internal async Task<RedisValue> GetFromMemoryAsync(string key, Func<Task<RedisValue>> retrieval)
        {
            return (await GetFromMemoryMultiAsync(new RedisKey[] { key }, async keys =>
            {
                var result = await retrieval().ConfigureAwait(false);
                return new[] { result };
            }).ConfigureAwait(false)).Single();
        }

        internal RedisValue[] GetFromMemoryMulti(RedisKey[] keys, Func<RedisKey[], RedisValue[]> retrieval)
        {
            if (!keys.Any()) return new RedisValue[0];

            var result = new RedisValue[keys.Length];
            var nonCachedIndices = new List<int>();

            for (var i = 0; i < keys.Length; i++)
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
            if (!nonCachedIndices.Any()) return result;


            var nonCachedKeys = keys.Where((key, index) => nonCachedIndices.Contains(index)).ToArray();
            var redisResults = retrieval(nonCachedKeys);
            if (redisResults == null) return result;

            var j = 0;
            foreach (var redisResult in redisResults)
            {
                var originalIndex = nonCachedIndices[j++];
                result[originalIndex] = redisResult;

                //Cache this key for next time
                try
                {
                    _memCache.Add(keys[originalIndex], redisResult, null, When.Always);
                }
                catch
                {
                }
            }

            return result;
        }

        internal async Task<RedisValue[]> GetFromMemoryMultiAsync(RedisKey[] keys, Func<RedisKey[], Task<RedisValue[]>> retrieval)
        {
            if (!keys.Any()) return new RedisValue[0];

            var result = new RedisValue[keys.Length];
            var nonCachedIndices = new List<int>();

            for (var i = 0; i < keys.Length; i++)
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
            if (!nonCachedIndices.Any()) return result;


            var nonCachedKeys = keys.Where((key, index) => nonCachedIndices.Contains(index)).ToArray();
            var redisResults = await retrieval(nonCachedKeys).ConfigureAwait(false);
            if (redisResults == null) return result;

            var j = 0;
            foreach (var redisResult in redisResults)
            {
                var originalIndex = nonCachedIndices[j++];
                result[originalIndex] = redisResult;

                //Cache this key for next time
                try
                {
                    _memCache.Add(keys[originalIndex], redisResult, null, When.Always);
                }
                catch
                {
                }
            }

            return result;
        }

        internal long AppendToString(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var existingValue = _memCache.Get<RedisValue>(key);
                if (existingValue.HasValue)
                {
                    //Get it and append it
                    string newValue = existingValue.Value + value;
                    _memCache.Update(key, (RedisValue)newValue);

                    return newValue.Length;
                }
                else
                {
                    //Create a new string
                    _memCache.Add(key, (RedisValue)value, null, When.Always);
                    return value.Length;
                }
            }

            return 0;
        }
    }
}
