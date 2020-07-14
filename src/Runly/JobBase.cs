using Runly.Processing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// An internal base class for jobs. This class should not be extended directly.
	/// </summary>
	/// <remarks>Create a job by extending one of the Job classes in Runly.Processing instead of using this class directly.</remarks>
	public abstract class JobBase<TConfig, TItem> : JobBase<TConfig>, IItemSource<TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes the <see cref="JobBase{TConfig, TItem}"/>.
		/// </summary>
		/// <param name="config">The job's config.</param>
		protected JobBase(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the items that will be processed by the <see cref="Job"/>
		/// </summary>
		/// <remarks>When the items are an <see cref="IEnumerable{T}"/> wrapped in an <see cref="AsyncEnumerableWrapper{T}"/> a count is done before processing. This 
		/// may have an adverse effect on some lists of items. Set canBeCounted on <see cref="AsyncEnumerableExtensions.ToAsyncEnumerable{T}(IEnumerable{T}, bool)"/> to false to disable this behavior.</remarks>
		public abstract IAsyncEnumerable<TItem> GetItemsAsync();

		/// <summary>
		/// Gets a unique ID for an item.
		/// </summary>
		/// <remarks>The default implementation uses item.ToString().</remarks>
		public virtual Task<string> GetItemIdAsync(TItem item) => Task.FromResult(item.ToString());
	}

	/// <summary>
	/// An internal base class for jobs. This class should not be extended directly.
	/// </summary>
	/// <remarks>Create a job by extending one of the Job classes in Runly.Processing instead of using this class directly.</remarks>
	public abstract class JobBase<TConfig> : IJob
		where TConfig : Config
	{
		/// <summary>
		/// Gets the config
		/// </summary>
		public TConfig Config { get; private set; }

		/// <summary>
		/// Allows the job to modify how it is executed.
		/// </summary>
		public JobOptions Options { get; } = new JobOptions();

		/// <summary>
		/// Gets a <see cref="CancellationToken"/> that can be used in <see cref="InitializeAsync"/>, ProcessAsync and others methods of a process.
		/// </summary>
		public CancellationToken CancellationToken { get; set; }

		Config IJob.Config => this.Config;

		/// <summary>
		/// Initializes the <see cref="JobBase{TConfig}"/>.
		/// </summary>
		/// <param name="config">The job's config.</param>
		protected JobBase(TConfig config)
		{
			this.Config = config ?? throw new ArgumentNullException(nameof(config));
		}

		Config IJob.GetDefaultConfig() => default(TConfig);

		Execution IJob.GetExecution(IServiceProvider provider) => GetExecution(provider);

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation for this job.
		/// </summary>
		/// <param name="provider">The <see cref="IServiceProvider"/> used to get depdendencies.</param>
		/// <returns>An <see cref="Execution"/>.</returns>
		protected internal abstract Execution GetExecution(IServiceProvider provider);

		/// <summary>
		/// Initializes the job before GetItemsAsync, ProcessAsync or <see cref="FinalizeAsync(Disposition)"/> are called.
		/// </summary>
		public virtual Task InitializeAsync() => Task.CompletedTask;

		/// <summary>
		/// Finalizes the job after all other methods have completed.
		/// </summary>
		/// <remarks>Override this method to perform tasks after all items have been processed.</remarks>
		/// <param name="disposition">The final state of the job run.</param>
		public virtual async Task<object> FinalizeAsync(Disposition disposition)
		{
			return disposition switch
			{
				Disposition.Successful => await OnCompletedAsync(),

				Disposition.Cancelled => await OnCancelledAsync(),

				Disposition.Failed => await OnFailedAsync(),

				_ => throw new NotSupportedException($"Unknown disposition: {disposition.ToString()}"),
			};
		}

		/// <summary>
		/// Called from <see cref="JobBase{TConfig}.FinalizeAsync(Disposition)"/> when the disposition is <see cref="Disposition.Successful"/>.
		/// </summary>
		public virtual Task<object> OnCompletedAsync() => Task.FromResult<object>(null);
		
		/// <summary>
		/// Called from <see cref="JobBase{TConfig}.FinalizeAsync(Disposition)"/> when the disposition is <see cref="Disposition.Cancelled"/>.
		/// </summary>
		public virtual Task<object> OnCancelledAsync() => Task.FromResult<object>(null);

		/// <summary>
		/// Called from <see cref="JobBase{TConfig}.FinalizeAsync(Disposition)"/> when the disposition is <see cref="Disposition.Failed"/>.
		/// </summary>
		public virtual Task<object> OnFailedAsync() => Task.FromResult<object>(null);
	}
}
