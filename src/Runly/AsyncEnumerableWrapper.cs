﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// Adapts an <see cref="IEnumerable{T}"/> to the <see cref="IAsyncEnumerable{T}"/> interface.
	/// </summary>
	/// <typeparam name="T">The type of item in the collection.</typeparam>
	public class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
	{
		readonly Task<IEnumerable<T>> enumerable;
		IEnumerable<T> innerEnumerable;

		/// <summary>
		/// Indicates whether the <see cref="AsyncEnumerableWrapper{T}"/> can be enumerated more than once.
		/// </summary>
		public bool CanBeCounted { get; }

		/// <summary>
		/// Creates a new <see cref="AsyncEnumerableWrapper{T}"/> with an <paramref name="enumerable"/> that will
		/// be awaited when the wrapper is enumerated.
		/// </summary>
		/// <param name="enumerable">The <see cref="IEnumerable{T}"/> to wrap.</param>
		/// <param name="canBeCounted">Indicates whether the <paramref name="enumerable"/> can be enumerated
		/// in order to count the items, then enumerated a second time to process the items.</param>
		public AsyncEnumerableWrapper(Task<IEnumerable<T>> enumerable, bool canBeCounted)
		{
			this.enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
			this.CanBeCounted = canBeCounted;
		}

		/// <summary>
		/// Creates a new <see cref="AsyncEnumerableWrapper{T}"/> from the <paramref name="enumerable"/>.
		/// </summary>
		/// <param name="enumerable">The <see cref="IEnumerable{T}"/> to wrap.</param>		
		/// <param name="canBeCounted">Indicates whether the <paramref name="enumerable"/> can be enumerated 
		/// in order to count the items, then enumerated a second time to process the items.</param>
		public AsyncEnumerableWrapper(IEnumerable<T> enumerable, bool canBeCounted)
		{
			_ = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
			this.enumerable = Task.FromResult(enumerable);
			this.innerEnumerable = enumerable;
			this.CanBeCounted = canBeCounted;
		}

		/// <summary>
		/// Gets the inner <see cref="IEnumerable{T}"/> being adapted to the <see cref="IAsyncEnumerable{T}"/> interface.
		/// </summary>
		public async Task<IEnumerable<T>> GetInnerEnumerable()
		{
			if (innerEnumerable == null)
				innerEnumerable = await enumerable;

			return innerEnumerable;
		}

		/// <summary>
		/// Gets an <see cref="IAsyncEnumerator{T}"/> to enumerate the items.
		/// </summary>
		/// <returns>An <see cref="AsyncEnumeratorWrapper{T}"/> as an <see cref="IAsyncEnumerator{T}"/>.</returns>
		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return new AsyncEnumeratorWrapper<T>(this);
		}
	}
}
