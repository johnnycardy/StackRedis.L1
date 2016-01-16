using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache.Types.SortedSet
{
    /// <summary>
    /// Represents a collection of disjointed parts of a sorted set.
    /// This means that individual continuous subsets of a redis Sorted Set can be stored in memory.
    /// </summary>
    internal class DisjointedSortedSet
    {
        private SortedSet<SortedSetRange> _ranges;

        /// <summary>
        /// Gets the total number of values stored in memory.
        /// </summary>
        internal int Count
        {
            get { return _ranges.Sum(r => r.Count); }
        }

        internal DisjointedSortedSet()
        {
            _ranges = new SortedSet<SortedSetRange>(new SortedSetRangeScoreComparer());
        }
        
        /// <summary>
        /// Adds what is assumed to be continuous range of entries, as returned by a redis call.
        /// Items from the array that are already present will be ignored.
        /// </summary>
        internal void Add(SortedSetEntry[] sortedContinuousEntries)
        {
            //Precondition: no two existing ranges contain overlapping scores.

            List<SortedSetRange> rangesToMerge = new List<SortedSetRange>();

            if (sortedContinuousEntries != null && sortedContinuousEntries.Any())
            {
                //Remove the items from existing ranges
                foreach(var entry in sortedContinuousEntries)
                {
                    foreach(var range in _ranges)
                    {
                        if (range.Remove(entry.Element))
                            break;
                    }
                }

                double firstNewScore = sortedContinuousEntries.First().Score;
                double lastNewScore = sortedContinuousEntries.Last().Score;

                //Find all of the current ranges which will be merged by this new range
                foreach (SortedSetRange range in _ranges)
                {
                    if (range.ScoreBelongs(firstNewScore) || range.ScoreBelongs(lastNewScore))
                    {
                        rangesToMerge.Add(range);
                    }
                    else if(firstNewScore < range.ScoreStart && lastNewScore > range.ScoreEnd)
                    {
                        //Check for the scenario where the supplied entries encompass entirely one or more ranges
                        rangesToMerge.Add(range);
                    }
                }
            }

            if(rangesToMerge.Any())
            {
                //Add all the new elements to the first range
                foreach (SortedSetEntry entry in sortedContinuousEntries)
                    rangesToMerge.First().Add(entry);

                //Now merge all the contents of the subsequent ranges back into the first.
                foreach (SortedSetEntry entry in rangesToMerge.Skip(1).SelectMany(r => r.Elements))
                    rangesToMerge.First().Add(entry);
            }
            else
            {
                _ranges.Add(new SortedSetRange(sortedContinuousEntries));
            }
        }

        internal void Remove(RedisValue[] entries)
        {
            List<RedisValue> toRemove = entries.ToList();

            foreach (var range in _ranges)
            {
                foreach(var redisValue in toRemove.ToArray())
                {
                    if (range.Remove(redisValue))
                        toRemove.Remove(redisValue);
                }
            }
        }

        internal void RemoveByScore(double start, double end, Exclude exclude)
        {
            foreach (var range in _ranges)
            {
                range.RemoveByScore(start, end, exclude);
            }
        }

        internal double? RetrieveScoreByValue(RedisValue value)
        {
            foreach (var range in _ranges)
            {
                foreach (var elem in range.Elements)
                {
                    if (elem.Element == value)
                        return elem.Score;
                }
            }

            return null;
        }

        /// <summary>
        /// If there is a continuous range in memory which includes the given scores, then it is returned - even if that is an empty range.
        /// Otherwise null is returned.
        /// </summary>
        internal IEnumerable<SortedSetEntry> RetrieveByScore(double start, double end, Exclude exclude)
        {
            foreach(var range in _ranges)
            {
                if(range.ScoreBelongs(start) && range.ScoreBelongs(end))
                {
                    return range.Subrange(start, end, exclude);
                }
            }

            return null;
        }
    }

    internal class SortedSetRangeScoreComparer : IComparer<SortedSetRange>
    {
        public int Compare(SortedSetRange x, SortedSetRange y)
        {
            //Note that the IComparer should never consider two ranges equal, since the set would discard one of them.
            return x.ScoreStart < x.ScoreStart ? -1 : 1;
        }
    }
}
