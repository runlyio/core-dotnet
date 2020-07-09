using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// Executes a <see cref="IJob"/>, communicating with the Runly API, handling exceptions, and providing status.
	/// </summary>
	public abstract class Execution
	{
		/// <summary>
		/// Event raised when execution of a job starts.
		/// </summary>
		public event Func<string, DateTime, Task> Started;

		/// <summary>
		/// Event raised when the state of the job changes.
		/// </summary>
		public event Func<ExecutionState, Task> StateChanged;

		/// <summary>
		/// Event raised when an processing an item completes.
		/// </summary>
		public event Func<ItemResult, Task> ItemCompleted;

		/// <summary>
		/// Event raised when a method completes.
		/// </summary>
		public event Func<MethodOutcome, Task> MethodCompleted;

		/// <summary>
		/// Event raised when execution of a job is complete.
		/// </summary>
		public event Func<object, Disposition, DateTime, Task> Completed;

		/// <summary>
		/// The <see cref="IJob"/> being executed.
		/// </summary>
		public abstract IJob Job { get; }

		/// <summary>
		/// The total number of items to be processed.
		/// </summary>
		/// <remarks>
		/// <see cref="TotalItemCount"/> will be null when a <see cref="IJob"/> does not use ToAsyncEnumerable with canCountItems 
		/// set to true. The difference between this total and the sum of <see cref="SuccessfulItemCount"/> and <see cref="FailedItemCount"/>
		/// is the number of items yet to be processed or not processed in the case of a job that ended in the <see cref="Disposition.Failed"/> state.
		/// </remarks>
		public int? TotalItemCount { get; protected set; }

		/// <summary>
		/// The number of items processed.
		/// </summary>
		public int CompletedItemCount => successfulItemCount + failedItemCount;

		int successfulItemCount, failedItemCount;

		/// <summary>
		/// The number of items processed with a successful (<see cref="Result.IsSuccessful"/>) <see cref="Result"/>.
		/// </summary>
		public int SuccessfulItemCount => successfulItemCount;

		/// <summary>
		/// The number of items processed with an unsuccessful (not <see cref="Result.IsSuccessful"/>) <see cref="Result"/>.
		/// </summary>
		public int FailedItemCount => failedItemCount;

		readonly ConcurrentDictionary<(string, bool), ItemCategory> categories = new ConcurrentDictionary<(string, bool), ItemCategory>();

		/// <summary>
		/// Gets the completed item category results.
		/// </summary>
		public IEnumerable<ItemCategory> ItemCategories => categories.Values;

		/// <summary>
		/// Gets the <see cref="ExecutionState">state</see> of execution.
		/// </summary>
		public ExecutionState State { get; private set; }

		/// <summary>
		/// The UTC time at which the run started.
		/// </summary>
		public DateTime? StartedAt { get; internal set; }

		/// <summary>
		/// The UTC time at which the run ended.
		/// </summary>
		public DateTime? CompletedAt { get; internal set; }

		/// <summary>
		/// Indicates whether the execution has completed.
		/// </summary>
		public bool IsComplete => CompletedAt.HasValue;

		/// <summary>
		/// The final state of the job.
		/// </summary>
		public Disposition Disposition =>
			IsFailed ? Disposition.Failed :
			IsCanceled ? Disposition.Cancelled :
			Disposition.Successful;

		/// <summary>
		/// Indicates whether the job has been cancelled or is in the process of cancelling.
		/// </summary>
		public bool IsCanceled => CanceledAt.HasValue;

		/// <summary>
		/// The UTC time at which execution of the job was cancelled.
		/// </summary>
		public DateTime? CanceledAt { get; private set; }

		/// <summary>
		/// Indicates whether the job has failed or in the process of failing. 
		/// </summary>
		public bool IsFailed => FailedAt.HasValue;

		/// <summary>
		/// The UTC time at which execution of the job failed.
		/// </summary>
		public DateTime? FailedAt { get; private set; }

		/// <summary>
		/// The <see cref="JobMethod">method</see> that was executing when the job failed.
		/// </summary>
		public JobMethod? FailedIn { get; private set; }

		/// <summary>
		/// The exception that caused the job to fail.
		/// </summary>
		public Exception FailedBecauseOf { get; private set; }

		CancellationTokenSource jobCancellation;

		/// <summary>
		/// Executes the <see cref="Job"/>.
		/// </summary>
		/// <param name="token">Optional <see cref="CancellationToken"/></param>
		public async Task ExecuteAsync(CancellationToken token)
		{
			jobCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
			Job.CancellationToken = jobCancellation.Token;
			Job.CancellationToken.Register(new Action(OnCancel));

			CancellationTokenSource eventPumpSource = new CancellationTokenSource();
			var eventPump = RunEventPump(eventPumpSource.Token);

			await ExecuteInternalAsync(token);

			eventPumpSource.Cancel();
			await eventPump;

			// Make sure all the events are raised
			await PublishEvents();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		protected abstract Task ExecuteInternalAsync(CancellationToken token);

		int cancelAfter;

		/// <summary>
		/// Causes the <see cref="Execution"/> to be cancelled once the <see cref="CompletedItemCount"/> reaches the <paramref name="numberOfItems"/> set.
		/// </summary>
		/// <remarks>Setting <paramref name="numberOfItems"/> to zero causes the job to execute normally.</remarks>
		public void CancelAfter(int numberOfItems)
		{
			this.cancelAfter = numberOfItems;
		}

		/// <summary>
		/// Cancels the execution of the job if the number of items processed has met or exceeded the cancellation threshold set by <see cref="CancelAfter(int)"/>.
		/// </summary>
		protected void EnforceCancelAfter()
		{
			if (cancelAfter > 0 && CompletedItemCount >= cancelAfter)
				jobCancellation.Cancel();
		}

		/// <summary>
		/// Initializes the <see cref="Execution"/>.
		/// </summary>
		protected Execution()
		{
			this.State = ExecutionState.NotStarted;
		}

		readonly ConcurrentQueue<object> events = new ConcurrentQueue<object>();

		class StartInfo
		{
			internal string Config { get; set; }
		}

		class CompleteInfo
		{
			internal object Output { get; set; }
		}

		class StateChange
		{
			internal ExecutionState State { get; set; }
		}

		/// <summary>
		/// Called internally to mark the start of the execution.
		/// </summary>
		/// <param name="config"></param>
		protected void Start(string config)
		{
			StartedAt = DateTime.UtcNow;
			events.Enqueue(new StartInfo() { Config = config });
		}

		/// <summary>
		/// Called internally to set the state of the execution.
		/// </summary>
		/// <param name="state"></param>
		protected void SetState(ExecutionState state)
		{
			if (State != state)
			{
				State = state;
				events.Enqueue(new StateChange() { State = state });
			}
		}

		/// <summary>
		/// Called internally to mark the completion of a call to a job method.
		/// </summary>
		protected void CompleteMethod(JobMethod method, TimeSpan duration, Exception exception)
		{
			events.Enqueue(new MethodOutcome(method, duration, exception));
		}

		/// <summary>
		/// Called internally to mark the completion of an item being processed.
		/// </summary>
		/// <param name="result"></param>
		protected void CompleteItem(ItemResult result)
		{
			if (result.IsSuccessful)
				Interlocked.Increment(ref successfulItemCount);
			else
				Interlocked.Increment(ref failedItemCount);

			var cat = categories.GetOrAdd((result.Category, result.IsSuccessful), new ItemCategory(0, result.IsSuccessful, result.Category));

			Interlocked.Increment(ref cat.count);

			events.Enqueue(result);
		}

		/// <summary>
		/// Called internally to mark the completion of the execution.
		/// </summary>
		protected void Complete(object output)
		{
			SetState(ExecutionState.Complete);
			CompletedAt = DateTime.UtcNow;
			events.Enqueue(new CompleteInfo() { Output = output });
		}

		private async Task RunEventPump(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await PublishEvents();
				
				await Task.Delay(100);
			}
		}

		private async Task PublishEvents()
		{
			object @event;

			while (events.TryDequeue(out @event))
			{
				try
				{
					await (@event switch
					{
						ItemResult result => ItemCompleted != null ? ItemCompleted(result) : Task.CompletedTask,
						MethodOutcome method => MethodCompleted != null ? MethodCompleted(method) : Task.CompletedTask,
						StateChange state => StateChanged != null ? StateChanged(state.State) : Task.CompletedTask,
						StartInfo start => Started != null ? Started(start.Config, StartedAt.Value) : Task.CompletedTask,
						CompleteInfo complete => Completed != null ? Completed(complete.Output, this.Disposition, CompletedAt.Value) : Task.CompletedTask,
						_ => throw new NotSupportedException(),
					});
				}
				catch
				{
					// Don't let an external event handler ruin our life.
					// Maybe this should be logged, but that adds a new dependency to this project.
				}
			}
		}

		private void OnCancel()
		{
			if (State != ExecutionState.Complete && !IsCanceled && !IsFailed)
				CanceledAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Called internally to complete the processing of an item and fail or cancel the job when appropriate.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="exception"></param>
		protected void CompleteProcessing(ItemResult result, Exception exception)
		{
			// CompleteItem needs to be called first, this increments FailedItemCount
			CompleteItem(result);

			if (FailedItemCount >= Math.Max(1, Job.Config.Execution.ItemFailureCountToStopJob))
				Fail(JobMethod.ProcessAsync, exception);

			EnforceCancelAfter();
		}

		/// <summary>
		/// Called internally to fail the execution when the <see cref="ExecutionConfig.ItemFailureCountToStopJob"/> threshold has been met or exceeded.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="failedIn"></param>
		/// <param name="exception"></param>
		protected void FailItem(ItemResult result, JobMethod failedIn, Exception exception)
		{
			CompleteItem(result);
			Fail(failedIn, exception);
		}

		/// <summary>
		/// Called internally to fail the execution when a job method throws an exception.
		/// </summary>
		protected void Fail(JobMethod failedIn, Exception exception)
		{
			if (!IsFailed)
			{
				FailedAt = DateTime.UtcNow;
				FailedIn = failedIn;
				FailedBecauseOf = exception;

				jobCancellation.Cancel();
			}
		}
	}
}