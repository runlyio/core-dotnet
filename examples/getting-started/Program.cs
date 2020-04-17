using System.Threading.Tasks;
using Examples.GettingStarted.Census;
using Runly;

namespace Examples.GettingStarted
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
