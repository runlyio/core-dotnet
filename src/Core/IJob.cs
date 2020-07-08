using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	public interface IJob
	{
		Config Config { get; }
		JobOptions Options { get; }
		CancellationToken CancellationToken { get; set; }
		Execution GetExecution(IServiceProvider provider);
		Config GetDefaultConfig();
		Task InitializeAsync();
		Task<object> FinalizeAsync(Disposition dispotion);
	}
}
