using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache.Types.SortedSet
{
    /// <summary>
    /// Represents a continuous range of items within a Redis SortedSet.
    /// </summary>
    internal class SortedSetRange
    {
        private SortedSet<SortedSetEntry> _sortedSet;
        
        internal IEnumerable<SortedSetEntry> Elements
        {
            get
            {
                return _sortedSet;
            }
        }

        /// <summary>
        /// Returns the score of the first item in this range
        /// </summary>
        public double ScoreStart
        {
            get
            {
                if (_sortedSet.Any())
                    return _sortedSet.First().Score;
                else
                    return double.MaxValue;
            }
        }

        /// <summary>
        /// Return the score of the last item in this range
        /// </summary>
        public double ScoreEnd
        {
            get
            {
                if (_sortedSet.Any())
                    return _sortedSet.Last().Score;
                else
                    return double.MinValue;
            }
        }

        internal SortedSetRange()
            : this(Enumerable.Empty<SortedSetEntry>())
        { }

        internal SortedSetRange(IEnumerable<SortedSetEntry> initialEntries)
        {
            _sortedSet = new SortedSet<SortedSetEntry>(initialEntries, new SortedSetScoreComparer());
        }

        /// <summary>
        /// Adds the entry to this range.
        /// </summary>
        internal void Add(SortedSetEntry entry)
        {
            _sortedSet.Add(entry);
        }

        /// <summary>
        /// Returns true if the supplied score belongs within this set range.
        /// </summary>
        internal bool ScoreBelongs(double score)
        {
            //If there's 0 or 1 items, it always belongs in this set.
            return _sortedSet.Count < 2 || (score >= ScoreStart && score <= ScoreEnd);
        }

        /// <summary>
        /// Gets a subrange of this sorted set range.
        /// </summary>
        internal IEnumerable<SortedSetEntry> Subrange(double start, double end)
        {
            foreach(var entry in _sortedSet)
            {
                if(entry.Score >= start)
                {
                    if(entry.Score <= end)
                    {
                        yield return entry;
                    }
                    else
                    {
                        //Since the set is ordered, we know we've reached the end of the subset.
                        break;
                    }
                }
            }
        }
        
    }

    internal class SortedSetScoreComparer : IComparer<SortedSetEntry>
    {
        public int Compare(SortedSetEntry x, SortedSetEntry y)
        {
            if (x.Score == y.Score && x.Element == y.Element)
            {
                return 0; //They're equal
            }
            else
            {
                return x.Score < y.Score ? -1 : 1;
            }
        }
    }
}
