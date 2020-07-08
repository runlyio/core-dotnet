using Runly;
using System.Threading.Tasks;

namespace NewRunlyApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await JobHost.CreateDefaultBuilder(args)
				.Build()
				.RunJobAsync();
		}
	}
}
