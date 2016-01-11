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

        internal DisjointedSortedSet()
        {
            _ranges = new SortedSet<SortedSetRange>(new SortedSetRangeScoreComparer());
        }

        /// <summary>
        /// Adds what is assumed to be a sorted, continuous range of entries, as returned by a redis call.
        /// </summary>
        internal void Add(SortedSetEntry[] sortedContinuousEntries)
        {
            //Precondition: no two existing ranges contain overlapping scores.

            List<SortedSetRange> rangesToMerge = new List<SortedSetRange>();

            if (sortedContinuousEntries != null && sortedContinuousEntries.Any())
            {
                double firstNewScore = sortedContinuousEntries.First().Score;
                double lastNewScore = sortedContinuousEntries.Last().Score;

                //Find all of the current ranges which will be merged by this new range
                foreach (SortedSetRange range in _ranges)
                {
                    if (range.ScoreBelongs(firstNewScore) || range.ScoreBelongs(lastNewScore))
                    {
                        rangesToMerge.Add(range);
                    }
                    else
                    {
                        //Check for the scenario where the supplied entries encompass entirely one or more ranges
                        if(firstNewScore < range.ScoreStart && lastNewScore > range.ScoreEnd)
                        {
                            rangesToMerge.Add(range);
                        }
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

        /// <summary>
        /// If there is a continuous range in memory which includes the given scores, then it is returned - even if that is an empty range.
        /// Otherwise null is returned.
        /// </summary>
        internal IEnumerable<SortedSetEntry> RetrieveByScore(double start, double end)
        {
            foreach(var range in _ranges)
            {
                if(range.ScoreBelongs(start) && range.ScoreBelongs(end))
                {
                    return range.Subrange(start, end);
                }
            }

            return null;
        }
    }

    internal class SortedSetRangeScoreComparer : IComparer<SortedSetRange>
    {
        public int Compare(SortedSetRange x, SortedSetRange y)
        {
            return x.ScoreStart < x.ScoreStart ? -1 : 1;
        }
    }
}
