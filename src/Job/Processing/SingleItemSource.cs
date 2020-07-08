using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Runly.Processing
{
	public class SingleItemSource : IItemSource<string>
	{
		public bool CanCountItems => true;

		public Task<string> GetItemIdAsync(string item) => Task.FromResult(item);

		public IAsyncEnumerable<string> GetItemsAsync() => (IAsyncEnumerable<string>)new string[] { "PROCESS" }.AsEnumerable();
	}
}
