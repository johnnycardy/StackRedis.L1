using StackExchange.Redis;
using StackRedis.L1.Notifications;
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
        private object _opLockObj = new object();

        private List<string> _markers = new List<string>();

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
        /// Given a continuous set of values, they are marked continuous in this disjointed sorted set.
        /// But, only if they're all already present.
        /// </summary>
        /// <param name="values"></param>
        internal void JoinRanges(RedisValue[] values)
        {
            if (values == null || values.Length <= 1)
                return;

            lock(_opLockObj)
            {
                SortedSet<SortedSetRange> existingRangesToMerge = new SortedSet<SortedSetRange>(new SortedSetRangeScoreComparer());

                int valueIndex = 0;
                bool startedValueMatching = false;

                foreach (var value in EnumerateValues())
                {
                    if (valueIndex >= values.Length)
                        break;

                    if (value.Item2.Element == values[valueIndex])
                    {
                        startedValueMatching = true;
                        existingRangesToMerge.Add(value.Item1); //Store the range
                        valueIndex++;
                    }
                    else
                    {
                        if (startedValueMatching)
                        {
                            //At this point, we've found some values but the range contains extra unexpected ones.
                            return;
                        }
                    }
                }

                //If all values were found, merge the ranges.
                if (startedValueMatching)
                {
                    SortedSetRange monsterRange = new SortedSetRange();
                    foreach (var rangeToMerge in existingRangesToMerge)
                    {
                        monsterRange.Add(rangeToMerge);
                        _ranges.Remove(rangeToMerge);
                    }

                    _ranges.Add(monsterRange);
                }
            }
        }

        private SortedSetEntry GetMarker(double score)
        {
            string marker = Guid.NewGuid().ToString();
            _markers.Add(marker);
            return new SortedSetEntry(marker, score);
        }

        /// <summary>
        /// Adds what is assumed to be continuous range of entries, as returned by a redis call.
        /// Items from the array that are already present will be ignored.
        /// Spanned existing ranges will be joined.
        internal void Add(SortedSetEntry[] entries)
        {
            if (entries.Any())
            {
                double min = entries.First().Score;
                double max = entries.Last().Score;

                Add(entries, min, max);
            }
        }
        
        /// <summary>
        /// Adds what is assumed to be continuous range of entries, as returned by a redis call.
        /// Items from the array that are already present will be ignored.
        /// Spanned existing ranges will be joined.
        /// the knownMin and knownMax can be smaller than the smallest score and larger than the largest score, respectively.
        /// - it will then be recorded that those are the real bounds.
        /// </summary>
        internal void Add(SortedSetEntry[] entries, double knownMin, double knownMax)
        {
            //Precondition: no two existing ranges contain overlapping scores.

            List<SortedSetRange> rangesToMerge = new List<SortedSetRange>();

            lock(_opLockObj)
            {
                if (entries != null && entries.Any())
                {
                    //Remove the items from existing ranges
                    Remove(entries.Select(e => e.Element).ToArray());

                    double firstNewScore = entries.First().Score;
                    double lastNewScore = entries.Last().Score;

                    List<SortedSetEntry> sortedContinuousEntries = entries.ToList();

                    //Add a dummy minimum item
                    if (firstNewScore > knownMin)
                        sortedContinuousEntries.Insert(0, GetMarker(knownMin));

                    //Add a dummy maximum item
                    if (lastNewScore < knownMax)
                        sortedContinuousEntries.Add(GetMarker(knownMax));

                    //Find all of the current ranges which will be merged by this new range
                    foreach (SortedSetRange range in _ranges)
                    {
                        if (range.ScoreBelongs(firstNewScore) || range.ScoreBelongs(lastNewScore))
                        {
                            rangesToMerge.Add(range);
                        }
                        else if (firstNewScore < range.ScoreStart && lastNewScore > range.ScoreEnd)
                        {
                            //Check for the scenario where the supplied entries encompass entirely one or more ranges
                            rangesToMerge.Add(range);
                        }
                    }
                    
                    if (rangesToMerge.Any())
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
                        //In this case there are no suitable ranges so create a new one.
                        _ranges.Add(new SortedSetRange(sortedContinuousEntries));
                    }
                }
            }
        }

        private IEnumerable<Tuple<SortedSetRange, SortedSetEntry>> EnumerateValues()
        {
            foreach (var range in _ranges)
            {
                foreach (var value in range.Elements)
                    yield return new Tuple<SortedSetRange, SortedSetEntry>(range, value);
            }
        }

        internal SortedSetEntry? RetrieveEntry(RedisValue entry)
        {
            foreach (var val in EnumerateValues())
            {
                if (val.Item2.Element == entry)
                    return val.Item2;
            }

            return null;
        }

        internal SortedSetEntry? RetrieveEntryByHashCode(int hashCode)
        {
            foreach(var val in EnumerateValues())
            {
                if (RedisValueHashCode.GetStableHashCode(val.Item2.Element) == hashCode)
                    return val.Item2;
            }

            return null;
        }

        internal void Remove(RedisValue[] entries)
        {
            List<RedisValue> toRemove = entries.ToList();

            lock(_opLockObj)
            {
                foreach (var range in _ranges.ToArray())
                {
                    foreach (var redisValue in toRemove.ToArray())
                    {
                        if (range.Remove(redisValue))
                        {
                            toRemove.Remove(redisValue);

                            if (range.Count == 0)
                            {
                                _ranges.Remove(range);
                                break;
                            }

                            //If the range only contains markers...
                            if(range.Count < _markers.Count && range.Elements.All(e => _markers.Contains(e.Element)))
                            {
                                _ranges.Remove(range);
                                break;
                            }
                        }
                    }

                    if (!toRemove.Any())
                        return; //Quit early
                }
            }
        }

        internal void RemoveByScore(double start, double end, Exclude exclude)
        {
            lock(_opLockObj)
            {
                foreach (var range in _ranges)
                {
                    range.RemoveByScore(start, end, exclude);
                }
            }
        }

        internal double? RetrieveScoreByValue(RedisValue value)
        {
            lock(_opLockObj)
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
        }

        /// <summary>
        /// If there is a continuous range in memory which includes the given scores, then it is returned - even if that is an empty range.
        /// Otherwise null is returned.
        /// </summary>
        internal IEnumerable<SortedSetEntry> RetrieveByScore(double start, double end, Exclude exclude)
        {
            lock(_opLockObj)
            {
                foreach (var range in _ranges)
                {
                    if (range.ScoreBelongs(start) && range.ScoreBelongs(end))
                    {
                        return range.Subrange(start, end, exclude)
                                    .Where(e => !_markers.Contains(e.Element));
                    }
                }
                return null;
            }
        }
    }

    internal class SortedSetRangeScoreComparer : IComparer<SortedSetRange>
    {
        public int Compare(SortedSetRange x, SortedSetRange y)
        {
            //Note that the IComparer should never consider two ranges equal, since the set would discard one of them.
            if (x.ScoreStart == y.ScoreStart)
                return 0;
            else
                return x.ScoreStart < y.ScoreStart ? -1 : 1;
        }
    }
}
