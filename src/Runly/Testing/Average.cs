using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Runly.Testing
{
    public class Average
    {
        private readonly ConcurrentQueue<DataPoint> _dataPoints = new ConcurrentQueue<DataPoint>();
        private long _errorCount;
        public string Name { get; } = string.Empty;
        public Metric AllTime { get; } = new Metric();
        public Metric LastMinuteAverage { get; } = new Metric();
        public decimal ErrorRate => AllTime.Count > 0 ? _errorCount / (decimal)AllTime.Count : 0;
        public long ErrorCount => _errorCount;

        public Average(string name)
        {
            Name = name;
        }

        public void RecordSuccess(TimeSpan duration)
        {
            var dp = new DataPoint
            {
                Time = DateTimeOffset.Now,
                Milliseconds = (long)duration.TotalMilliseconds
            };

            _dataPoints.Enqueue(dp);

            AllTime.Add(dp);
            LastMinuteAverage.Add(dp);
        }

        public void RecordError()
        {
            Interlocked.Increment(ref _errorCount);
        }

        public void Purge()
        {
            DataPoint dp;

            while (_dataPoints.TryPeek(out dp))
            {
                if (dp.Time > DateTimeOffset.Now.AddMinutes(-1))
                    return;

                if (_dataPoints.TryDequeue(out dp))
                    LastMinuteAverage.Subtract(dp!);
            }
        }

        public override string ToString()
        {
            return $@"
{Name}
----------------------------
Last Min:   {LastMinuteAverage.Count}/{LastMinuteAverage.Average.TotalMilliseconds}ms avg
Total:      {AllTime.Count}/{AllTime.Average.TotalMilliseconds}ms avg
Errors:     {ErrorCount}/{string.Format("{0:P2}", ErrorRate)}

";
        }
    }
}
