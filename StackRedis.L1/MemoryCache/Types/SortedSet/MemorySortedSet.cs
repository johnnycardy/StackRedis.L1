using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache.Types.SortedSet
{
    internal class MemorySortedSet
    {
        private ObjMemCache _objMemCache;
        private object _setMoveLockObj = new object();

        internal MemorySortedSet(ObjMemCache objMemCache)
        {
            _objMemCache = objMemCache;
        }

        /// <summary>
        /// Adds entries which are *not* assumed to be continuous.
        /// </summary>
        internal void Add(string key, SortedSetEntry[] entries, bool itemsAreContinuous)
        {
            var set = GetSortedSet(key);
            if (set == null)
                set = SetSortedSet(key);

            if (itemsAreContinuous)
            {
                set.Add(entries);
            }
            else
            {
                //Add them separately as we don't know if they're continuous
                foreach (var entry in entries)
                    set.Add(new[] { entry });
            }
        }

        /// <summary>
        /// If the supplied values are already all present in disjointed sections of the set, then they will be joined.
        /// </summary>
        internal void MarkValuesAsContinuous(string key, RedisValue[] entries)
        {
            var set = GetSortedSet(key);
            if(set != null)
            {
                set.JoinRanges(entries);
            }
        }

        /// <summary>
        /// Removes the specified entries
        /// </summary>
        internal void Delete(string key, RedisValue[] entries)
        {
            var set = GetSortedSet(key);
            if (set != null)
            {
                set.Remove(entries);
            }
        }

        internal void RemoveByHashCode(string key, int hashCode)
        {
            var set = GetSortedSet(key);
            if (set != null)
            {
                var entry = set.RetrieveEntryByHashCode(hashCode);
                if(entry.HasValue)
                {
                    set.Remove(new[] { entry.Value.Element });
                }
            }
        }

        /// <summary>
        /// Gets the entries matching the specified hash code
        /// </summary>
        internal SortedSetEntry? GetByHashCode(string key, int hashCode)
        {
            var set = GetSortedSet(key);
            if(set != null)
            {
                return set.RetrieveEntryByHashCode(hashCode);
            }
            return null;
        }

        internal void DeleteByScore(string key, double start, double end, Exclude exclude)
        {
            var set = GetSortedSet(key);
            if(set != null)
            {
                set.RemoveByScore(start, end, exclude);
            }
        }

        internal SortedSetEntry? GetEntry(string key, RedisValue member)
        {
            var set = GetSortedSet(key);
            if (set != null)
            {
                var entry = set.RetrieveEntry(member);
                if(entry.HasValue)
                {
                    return entry.Value;
                }
            }

            return null;
        }

        internal IEnumerable<SortedSetEntry> GetByScore(string key, double start, double end, Exclude exclude)
        {
            var set = GetSortedSet(key);
            if (set != null)
            {
                return set.RetrieveByScore(start, end, exclude);
            }

            return null;
        }

        private DisjointedSortedSet SetSortedSet(string key)
        {
            var set = new DisjointedSortedSet();
            _objMemCache.Add(key, set, null, When.Always);
            return set;
        }

        private DisjointedSortedSet GetSortedSet(string key)
        {
            var result = _objMemCache.Get<DisjointedSortedSet>(key);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return null;
            }
        }

        internal void IncrementOrAdd(RedisKey key, RedisValue member, double value)
        {
            var set = GetSortedSet(key);
            if (set != null)
            {
                var existingScore = set.RetrieveScoreByValue(member);
                var newScore = existingScore.GetValueOrDefault(0) + value;

                if (existingScore.HasValue)
                {
                    set.Remove(new[] { member });
                }

                set.Add(new[] { new SortedSetEntry(member, newScore) });
            }
        }

        internal void DecrementOrAdd(RedisKey key, RedisValue member, double value)
        {
            var set = GetSortedSet(key);
            if(set != null)
            {
                var existingScore = set.RetrieveScoreByValue(member);
                var newScore = existingScore.GetValueOrDefault(0) - value;

                if (existingScore.HasValue)
                {
                    set.Remove(new[] { member });
                }

                set.Add(new[] { new SortedSetEntry(member, newScore) });
            }
        }
    }
}
