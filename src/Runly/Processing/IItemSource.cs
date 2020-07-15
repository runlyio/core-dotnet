using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly.Processing
{
	/// <summary>
	/// The source of items for a <see cref="IJob"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of item.</typeparam>
	public interface IItemSource<TItem>
	{
		/// <summary>
		/// Gets the items to process.
		/// </summary>
		/// <returns>An <see cref="IAsyncEnumerable{T}"/> of items.</returns>
		/// <remarks>
		/// When implementing this method, if the underlying data source returns an IEnumerable,
		/// use the extension method <see cref="AsyncEnumerableExtensions.ToAsyncEnumerable{T}(IEnumerable{T}, bool)"/>.
		/// </remarks>
		IAsyncEnumerable<TItem> GetItemsAsync();

		/// <summary>
		/// Gets a human readable identifier for the item. The default implementation uses <see cref="object.ToString"/>.
		/// </summary>
		/// <param name="item">The item to get an identifier for.</param>
		/// <returns>A human readable identifier.</returns>
		Task<string> GetItemIdAsync(TItem item);
	}
}
