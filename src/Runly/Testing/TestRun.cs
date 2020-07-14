using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Testing
{
	/// <summary>
	/// A test run of a job, built by a <see cref="TestHost{TJob}"/>.
	/// </summary>
	/// <typeparam name="TJob">The type of <see cref="IJob"/> being tested.</typeparam>
	public class TestRun<TJob> : IDisposable
		where TJob : IJob
	{
		CancellationTokenSource cts;

		/// <summary>
		/// The <see cref="IServiceProvider"/> used to run the <see cref="Job"/>.
		/// </summary>
		public IServiceProvider Services { get; }

		/// <summary>
		/// The job being tested.
		/// </summary>
		public TJob Job { get; }

		/// <summary>
		/// The <see cref="Execution"/> implementation that ran the job.
		/// </summary>
		public Execution Execution { get; }

		/// <summary>
		/// A log of results emitted from the job during execution.
		/// </summary>
		public ResultLog Results { get; }

		internal TestRun(TJob job, Execution execution, ResultLog results, ServiceProvider services) => (Job, Execution, Results, Services) = (job, execution, results, services);

		/// <summary>
		/// Performs clean up.
		/// </summary>
		public void Dispose() => (Services as IDisposable)?.Dispose();

		/// <summary>
		/// Triggers cancellation of a running job.
		/// </summary>
		public void Cancel()
		{
			if (cts == null)
				throw new InvalidOperationException($"{nameof(Cancel)} cannot be called before {nameof(RunAsync)}");

			cts.Cancel();
		}

		/// <summary>
		/// Runs the job.
		/// </summary>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> to trigger cancellation.</param>
		/// <param name="cancelAfter">When a positive value is given, cancels the job after that many items have been processed. This is useful to stop a service after an expected number of items have been processed in a unit test.</param>
		public async Task<TestRun<TJob>> RunAsync(CancellationToken cancellationToken = default, int cancelAfter = 0)
		{
			if (cts != null)
				throw new InvalidOperationException($"{nameof(RunAsync)} cannot be called more than once");

			cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			Execution.CancelAfter(cancelAfter);
			await Execution.ExecuteAsync(cts.Token);

			return this;
		}
	}
}
