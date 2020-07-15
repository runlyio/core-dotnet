using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Runly.Processing
{
	/// <summary>
	/// Used by <see cref="Job{TConfig}"/> to call <see cref="Job{TConfig}.ProcessAsync"/> a single time.
	/// </summary>
	internal class SingleItemSource : IItemSource<string>
	{
		public bool CanCountItems => true;

		public Task<string> GetItemIdAsync(string item) => Task.FromResult(item);

		public IAsyncEnumerable<string> GetItemsAsync() => (IAsyncEnumerable<string>)new string[] { nameof(Job<Config>.ProcessAsync) }.AsEnumerable();
	}
}
