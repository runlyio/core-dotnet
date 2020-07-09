using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	public class PollingStream<T> : IAsyncEnumerable<T>
	{
		readonly Func<IAsyncEnumerable<T>> poll;
		readonly TimeSpan interval;

		public PollingStream(Func<IAsyncEnumerable<T>> poll, TimeSpan interval)
		{
			this.poll = poll ?? throw new ArgumentNullException(nameof(poll));
			this.interval = interval;

			if (interval < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(interval), "Must be a non-negative TimeSpan");
		}

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new Enumerator(this, cancellationToken);
		}

		public class Enumerator : IAsyncEnumerator<T>
		{
			readonly PollingStream<T> pollingStream;
			readonly CancellationToken cancellationToken;
			IAsyncEnumerator<T> stream;
			DateTime lastPoll;

			public Enumerator(PollingStream<T> pollingStream, CancellationToken cancellationToken)
			{
				this.pollingStream = pollingStream;
				this.cancellationToken = cancellationToken;
			}

			public T Current => stream != null ? stream.Current : default;

			public async ValueTask<bool> MoveNextAsync()
			{
				var moveNext = false;

				if (stream != null)
					moveNext = await stream.MoveNextAsync();

				while (!moveNext || stream == null)
				{
					// Delay for the remainder of the polling interval
					var delay = lastPoll.Add(pollingStream.interval).Subtract(DateTime.UtcNow);

					if (stream != null && delay > TimeSpan.Zero)
						await Task.Delay(delay, cancellationToken)
							.ContinueWith(tsk => {/* Don't throw exception on cancellation https://stackoverflow.com/a/32768637 */});

					if (cancellationToken.IsCancellationRequested)
						return false;

					lastPoll = DateTime.UtcNow;
					var next = pollingStream.poll();

					if (next == null)
						return false;

					stream = next.GetAsyncEnumerator();

					moveNext = await stream.MoveNextAsync();
				}

				return moveNext;
			}

			public async ValueTask DisposeAsync()
			{
				if (stream != null)
					await stream.DisposeAsync();
			}
		}
	}
}
