using System.Threading.Tasks;

namespace Runly.Examples
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await ProcessHost.CreateDefaultBuilder(args)
				.Build()
				.RunProcessAsync();
		}
	}
}
