using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// Adapts an <see cref="IEnumerator{T}"/> to the <see cref="IAsyncEnumerator{T}"/> interface.
	/// </summary>
	/// <typeparam name="T">The type of item in the collection.</typeparam>
	public class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
	{
		readonly AsyncEnumerableWrapper<T> enumerable;
		readonly Task<IEnumerator<T>> getEnumerator;
		IEnumerator<T> enumerator;

		/// <summary>
		/// Creates a new <see cref="AsyncEnumeratorWrapper{T}"/> from the <see cref="AsyncEnumerableWrapper{T}"/>.
		/// </summary>
		/// <param name="enumerable">The <see cref="IEnumerator{T}"/> to wrap.</param>
		public AsyncEnumeratorWrapper(AsyncEnumerableWrapper<T> enumerable)
		{
			this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
		}

		/// <summary>
		/// Creates a new <see cref="AsyncEnumeratorWrapper{T}"/> from the <see cref="Task{IEnumerator{T}}"/>.
		/// </summary>
		/// <param name="enumerable">The <see cref="IEnumerator{T}"/> to wrap.</param>
		public AsyncEnumeratorWrapper(Task<IEnumerator<T>> enumerator)
		{
			this.getEnumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
		}

		/// <summary>
		/// Creates a new <see cref="AsyncEnumeratorWrapper{T}"/> from the <see cref="Task{IEnumerator{T}}"/>.
		/// </summary>
		/// <param name="enumerable">The <see cref="IEnumerator{T}"/> to wrap.</param>
		public AsyncEnumeratorWrapper(IEnumerator<T> enumerator)
		{
			this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
			this.getEnumerator = Task.FromResult(enumerator);
		}

		/// <summary>
		/// Gets the inner <see cref="IEnumerator{T}"/> being adapted to the <see cref="IAsyncEnumerator{T}"/> interface.
		/// </summary>
		public async Task<IEnumerator<T>> GetInnerEnumerator()
		{
			if (enumerator == null)
			{
				if (enumerable != null)
					enumerator = (await enumerable.GetInnerEnumerable()).GetEnumerator();
				else
					enumerator = await getEnumerator;
			}

			return enumerator;
		}

		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public T Current => enumerator != null ? enumerator.Current : default(T);

		/// <summary>
		/// Advances the enumerator asynchronously to the next element of the collection.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<bool> MoveNextAsync() => (await GetInnerEnumerator()).MoveNext();

		/// <summary>
		/// Disposes the <see cref="AsyncEnumeratorWrapper{T}"/>.
		/// </summary>
		public ValueTask DisposeAsync()
		{
			enumerator?.Dispose();

			return new ValueTask(Task.CompletedTask);
		}
	}
}
