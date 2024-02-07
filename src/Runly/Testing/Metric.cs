using System;
using System.Threading;

namespace Runly.Testing
{
    public class Metric
    {
        private long _count;
        private long _total;

        public long Count => _count;
        public long Total => _total;
        public TimeSpan Average => _count > 0 ? TimeSpan.FromMilliseconds(_total / _count) : TimeSpan.Zero;

        public void Add(DataPoint data)
        {
            Interlocked.Increment(ref _count);
            Interlocked.Add(ref _total, data.Milliseconds);
        }

        public void Subtract(DataPoint data)
        {
            Interlocked.Decrement(ref _count);
            Interlocked.Add(ref _total, -data.Milliseconds);
        }
    }
}
