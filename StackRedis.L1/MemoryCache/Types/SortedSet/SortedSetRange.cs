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
        
        internal int Count
        {
            get { return _sortedSet.Count; }
        }

        internal IEnumerable<SortedSetEntry> Elements
        {
            get { return _sortedSet; }
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
        
        internal bool Remove(RedisValue entry)
        {
            return _sortedSet.RemoveWhere(e => e.Element == entry) >= 0;
        }

        internal int RemoveByScore(double start, double end, Exclude exclude)
        {
            return _sortedSet.RemoveWhere(elem => IsMatch(elem.Score, start, end, exclude));
        }

        internal static bool IsMatch(double test, double start, double stop, Exclude exclude)
        {
            if(test == start && (exclude == Exclude.None || exclude == Exclude.Stop))
            {
                return true;
            }
            else if(test == stop && (exclude == Exclude.None || exclude == Exclude.Start))
            {
                return true;
            }
            else
            {
                return test > start && test < stop;
            }
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
            return score >= ScoreStart && score <= ScoreEnd;
        }

        /// <summary>
        /// Gets a subrange of this sorted set range.
        /// </summary>
        internal IEnumerable<SortedSetEntry> Subrange(double start, double end, Exclude exclude)
        {
            return _sortedSet.Where(elem => IsMatch(elem.Score, start, end, exclude));
        }
        
    }

    internal class SortedSetScoreComparer : IComparer<SortedSetEntry>
    {
        public int Compare(SortedSetEntry x, SortedSetEntry y)
        {
            if (x.Score == y.Score && x.Element == y.Element)
            {
                return 0;
            }
            else
            {
                return x.Score < y.Score ? -1 : 1;
            }
        }
    }
}
