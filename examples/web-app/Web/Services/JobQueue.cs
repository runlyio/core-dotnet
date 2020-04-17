using Examples.WebApp.Jobs;
using Examples.WebApp.Web.Config;
using Microsoft.Extensions.Options;
using Runly;
using System;
using System.Threading.Tasks;

namespace Examples.WebApp.Web.Services
{
	public interface IJobQueue
	{
		// provide a domain-specific method to queue jobs
		Task<Guid> SendPendingInvitations();
	}

	public class RunlyJobQueue : IJobQueue
	{
		readonly AppOptions appOpts;
		readonly RunlyOptions runlyOpts;
		readonly IRunClient runs;

		public RunlyJobQueue(
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
			// this method just abstracts the specifics of queueing a job away from the app
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

						// don't stop the job unless we get over 100 failed items
						ItemFailureCountToStopJob = 100
					}
				}
			);

			return run.Id;
		}
	}
}
