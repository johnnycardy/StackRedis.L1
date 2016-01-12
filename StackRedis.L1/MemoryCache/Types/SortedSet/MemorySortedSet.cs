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


    }
}
