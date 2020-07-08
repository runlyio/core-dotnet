using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly.Processing
{
	public interface IItemSource<TItem>
	{
		IAsyncEnumerable<TItem> GetItemsAsync();

		Task<string> GetItemIdAsync(TItem item);
	}
}
