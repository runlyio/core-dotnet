using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly
{
	public static class AsyncEnumerableExtensions
	{
		/// <summary>
		/// Wraps the <paramref name="enumerable"/> in an <see cref="AsyncEnumerableWrapper{T}"/> so that it can
		/// be used as an <see cref="IAsyncEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of list item</typeparam>
		/// <param name="enumerable">The <see cref="IEnumerable{T}"/> to adapt to the <see cref="IAsyncEnumerable{T}"/> interface.</param>
		/// <param name="canBeCounted">Indicates whether the collection can be counted before being enumerated. True by default.</param>
		/// <returns>An <see cref="IAsyncEnumerable{T}"/></returns>
		/// <remarks><paramref name="enumerable"/> is awaited at the first call to <see cref="IAsyncEnumerator{T}.MoveNextAsync"/> unless <see cref="AsyncEnumeratorWrapper{T}.GetInnerEnumerator"/> is called first.</remarks>
		public static AsyncEnumerableWrapper<T> ToAsyncEnumerable<T>(this Task<IEnumerable<T>> enumerable, bool canBeCounted = true)
		{
			return new AsyncEnumerableWrapper<T>(enumerable, canBeCounted);
		}

		/// <summary>
		/// Wraps the <paramref name="enumerable"/> in an <see cref="AsyncEnumerableWrapper{T}"/> so that it can
		/// be used as an <see cref="IAsyncEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of list item</typeparam>
		/// <param name="enumerable">The <see cref="IEnumerable{T}"/> to adapt to the <see cref="IAsyncEnumerable{T}"/> interface.</param>
		/// <param name="canBeCounted">Indicates whether the collection can be counted before being enumerated. True by default.</param>
		/// <returns>An <see cref="IAsyncEnumerable{T}"/></returns>
		public static AsyncEnumerableWrapper<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable, bool canBeCounted = true)
		{
			return new AsyncEnumerableWrapper<T>(enumerable, canBeCounted);
		}
	}
}
