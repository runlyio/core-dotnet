using Examples.WebApp.Processes;
using Examples.WebApp.Web.Config;
using Microsoft.Extensions.Options;
using Runly;
using System;
using System.Threading.Tasks;

namespace Examples.WebApp.Web.Services
{
	public interface IProcessQueue
	{
		// provide a domain-specific method to queue processes
		Task<Guid> SendPendingInvitations();
	}

	public class RunlyProcessQueue : IProcessQueue
	{
		readonly AppOptions appOpts;
		readonly RunlyOptions runlyOpts;
		readonly IRunClient runs;

		public RunlyProcessQueue(
			IOptionsSnapshot<AppOptions> appOpts,
			IOptionsSnapshot<RunlyOptions> runlyOpts,
			IRunClient runs
		)
		{
			this.appOpts = appOpts.Value;
			this.runlyOpts = runlyOpts.Value;
			this.runs = runs;
		}

		public async Task<Guid> SendPendingInvitations()
		{
			// this method just abstracts the specifics of queueing a process away from the app
			var run = await runs.Enqueue<InvitationEmailer, InvitationEmailerConfig>(
				runlyOpts.Org,
				runlyOpts.Env,
				new InvitationEmailerConfig
				{
					ConnectionString = appOpts.ConnectionString,
					Execution = new ExecutionConfig
					{
						// let's send 50 emails at a time
						ParallelTaskCount = 50,

						// don't stop the process unless we get over 100 failed items
						ItemFailureCountToStopProcess = 100
					}
				}
			);

			return run.Id;
		}
	}
}
