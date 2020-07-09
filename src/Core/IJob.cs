using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// The interface that all Runly jobs must implement.
	/// </summary>
	/// <remarks>
	/// Do not implement this interface directly, instead you should extend one of the Runly.Job classes in the Runly library.
	/// </remarks>
	public interface IJob
	{
		/// <summary>
		/// Gets the job's config.
		/// </summary>
		Config Config { get; }

		/// <summary>
		/// Gets the options for the job, determining how the job can be run.
		/// </summary>
		JobOptions Options { get; }

		/// <summary>
		/// Gets the <see cref="CancellationToken"/> that the job should use when running.
		/// </summary>
		CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation that can run the job.
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		Execution GetExecution(IServiceProvider provider);

		/// <summary>
		/// Gets the default config for the job.
		/// </summary>
		/// <returns></returns>
		Config GetDefaultConfig();

		/// <summary>
		/// Initializes the job, being called before any items are processed.
		/// </summary>
		/// <returns></returns>
		Task InitializeAsync();

		/// <summary>
		/// Finalizes the job, being called after all items have been processed.
		/// </summary>
		/// <param name="dispotion"></param>
		/// <returns></returns>
		Task<object> FinalizeAsync(Disposition dispotion);
	}
}
