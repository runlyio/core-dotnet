using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Testing
{
	public class TestRun<TJob>
		where TJob : IJob
	{
		CancellationTokenSource cts;

		public TJob Job { get; }
		public Execution Execution { get; }
		public ResultLog Results { get; }

		internal TestRun(TJob job, Execution execution, ResultLog results) => (Job, Execution, Results) = (job, execution, results);

		public void Cancel()
		{
			if (cts == null)
				throw new InvalidOperationException($"{nameof(Cancel)} cannot be called before {nameof(RunAsync)}");

			cts.Cancel();
		}

		/// <summary>
		/// Runs the job
		/// </summary>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> to use</param>
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
