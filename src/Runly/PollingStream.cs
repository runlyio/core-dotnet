using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// Calls a function that returns an enumerable collection on an interval, resulting in a stream of data.
	/// </summary>
	/// <typeparam name="T">The type of element in the collection.</typeparam>
	public class PollingStream<T> : IAsyncEnumerable<T>
	{
		readonly Func<IAsyncEnumerable<T>> poll;
		readonly TimeSpan interval;

		/// <summary>
		/// Initializes a new <see cref="PollingStream{T}"/>.
		/// </summary>
		/// <param name="poll">The poll function to call.</param>
		/// <param name="interval">The interval on which to call the poll function.</param>
		public PollingStream(Func<IAsyncEnumerable<T>> poll, TimeSpan interval)
		{
			this.poll = poll ?? throw new ArgumentNullException(nameof(poll));
			this.interval = interval;

			if (interval < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(interval), "Must be a non-negative TimeSpan");
		}

		/// <summary>
		/// Returns an enumerator that iterates asynchronously through the collection.
		/// </summary>
		/// <param name="cancellationToken">The token to trigger cancellation.</param>
		/// <returns>An <see cref="IAsyncEnumerator{T}"/>.</returns>
		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new Enumerator(this, cancellationToken);
		}

		/// <summary>
		/// An enumerator that calls the poll function of <see cref="PollingStream{T}"/> on an interval.
		/// </summary>
		public class Enumerator : IAsyncEnumerator<T>
		{
			readonly PollingStream<T> pollingStream;
			readonly CancellationToken cancellationToken;
			IAsyncEnumerator<T> stream;
			DateTime lastPoll;

			/// <summary>
			/// Initializes a new <see cref="PollingStream{T}.Enumerator"/>.
			/// </summary>
			/// <param name="pollingStream">The <see cref="PollingStream{T}"/> to enumerate.</param>
			/// <param name="cancellationToken">The token to trigger cancellation.</param>
			public Enumerator(PollingStream<T> pollingStream, CancellationToken cancellationToken)
			{
				this.pollingStream = pollingStream;
				this.cancellationToken = cancellationToken;
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			public T Current => stream != null ? stream.Current : default;

			/// <summary>
			/// Advances the enumerator asynchronously to the next element of the collection.
			/// </summary>
			/// <returns>True if the enumerator advanced.</returns>
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

			/// <summary>
			/// Disposes of resources.
			/// </summary>
			public async ValueTask DisposeAsync()
			{
				if (stream != null)
					await stream.DisposeAsync();
			}
		}
	}
}
