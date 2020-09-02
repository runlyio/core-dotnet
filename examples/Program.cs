using System.Threading.Tasks;
using Examples.GettingStarted.Census;
using Runly;

namespace Examples.GettingStarted
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await JobHost.CreateDefaultBuilder(args)
				.ConfigureServices((host, services) =>
				{
					// We can register any dependencies our jobs need here.
					services.AddCensusJobs();
				})
				.Build()
				.RunJobAsync();
		}
	}
}
