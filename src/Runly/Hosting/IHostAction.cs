using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Hosting
{
	interface IHostAction
	{
		Task RunAsync(CancellationToken cancel);
	}

	static class HostActionExtensions
	{
		public static Task RunAsync(this IHostAction action)
		{
			return RunAsync(action, new CancellationTokenSource().Token);
		}

		public static Task RunAsync(this IHostAction action, CancellationToken cancellation)
		{
			return action.RunAsync(cancellation);
		}
	}
}
