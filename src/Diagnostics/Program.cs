using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Runly.Diagnostics
{
	class Program
	{
		static Task Main(string[] args)
		{
			return JobHost.CreateDefaultBuilder(args)
				.Build()
				.RunAsync();
		}
	}
}
