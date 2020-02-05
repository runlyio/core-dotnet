using System.Threading.Tasks;
using Runly.Examples.Census;

namespace Runly.Examples
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await ProcessHost.CreateDefaultBuilder(args)
				.ConfigureServices((host, services) =>
				{
					// We can register any dependencies our processes need here.
					services.AddCensusProcesses();
				})
				.Build()
				.RunProcessAsync();
		}
	}
}
