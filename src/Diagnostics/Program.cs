using System.Threading.Tasks;

namespace Runly.Diagnostics
{
	class Program
	{
		static Task Main(string[] args)
		{
			return JobHost.CreateDefaultBuilder(args)
				.Build()
				.RunJobAsync();
		}
	}
}
