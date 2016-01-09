using StackExchange.Redis;
using StackRedis.L1.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache.Types
{
    internal class MemorySets
    {
        private ObjMemCache _objMemCache;
        private object _setMoveLockObj = new object();

        internal MemorySets(ObjMemCache objMemCache)
        {
            _objMemCache = objMemCache;
        }

        /// <summary>
        /// Returns true if there's a set in memory that contains the specified item.
        /// </summary>
        internal bool Contains(string setKey, RedisValue value)
        {
            var set = GetSet(setKey);
            return set != null && set.Contains(value);
        }

        /// <summary>
        /// Moves an item from one set to another
        /// </summary>
        internal bool Move(string source, string destination, RedisValue value)
        {
            //Locking to avoid two opposing SetMove operations from interacting (which could result in the value being lost in-memory or worse, exist in two sets)
            lock (_setMoveLockObj)
            {
                bool removed = Remove(source, new[] { value }) > 0;
                if (removed)
                {
                    Add(destination, new[] { value });
                    return true;
                }

                return false;
            }
        }

        internal long Remove(string setKey, RedisValue[] values)
        {
            long result = 0;
            var set = GetSet(setKey);
            if (set != null)
            {
                foreach (RedisValue value in values)
                {
                    if (set.Contains(value))
                    {
                        set.Remove(value);
                        result++;
                    }
                }
            }

            return result;
        }


        //Remove entries given a hash code
        internal void RemoveByHashCode(string key, string[] hashCodes)
        {
            var set = GetSet(key);
            if(set != null)
            {
                //Get the items for the supplied hash codes
                foreach(var hashCode in hashCodes)
                {
                    var value = set.FirstOrDefault(v => RedisValueHashCode.GetStableHashCode(v).ToString() == hashCode);
                    if(value != default(RedisValue))
                    {
                        set.Remove(value);
                    }
                }
            }
        }

        internal long Add(string setKey, RedisValue[] values)
        {
            long result = 0;
            var set = GetSet(setKey);
            if (set == null)
                set = AddSet(setKey);

            foreach (var value in values)
            {
                if (!set.Contains(value))
                {
                    set.Add(value);
                    result++;
                }
            }

            return result;
        }

        private HashSet<RedisValue> AddSet(string setKey)
        {
            HashSet<RedisValue> result = new HashSet<RedisValue>();

            _objMemCache.Add(setKey, result, null, When.Always);

            return result;
        }

        private HashSet<RedisValue> GetSet(string setKey)
        {
            var result = _objMemCache.Get<HashSet<RedisValue>>(setKey);
            if (result.HasValue)
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
