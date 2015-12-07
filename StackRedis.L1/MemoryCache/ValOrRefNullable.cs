using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.MemoryCache
{
    public class ValOrRefNullable<T>
    {
        public bool HasValue { get; private set; }
        public T Value { get; private set; }

        public ValOrRefNullable()
        {
            HasValue = false;
        }

        public ValOrRefNullable(T value)
        {
            HasValue = true;
            Value = value;
        }
    }
}
