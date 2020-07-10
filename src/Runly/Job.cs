using Runly.Processing;
using System;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// A configurationless, single-action asynchronous job.
	/// </summary>
	public abstract class Job : Job<Config>
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(Config config)
			: base(config) { }
	}

	/// <summary>
	/// A single-action asynchronous job.
	/// </summary>
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	public abstract class Job<TConfig> : JobBase<TConfig>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig>(this, provider);

		/// <summary>
		/// Performs the processing for the job.
		/// </summary>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync();
	}

	/// <summary>
	/// A job that provides a list of items and an action to perform on each item.
	/// </summary>
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	public abstract class Job<TConfig, TItem> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item);
	}

	/// <summary>
	/// A job with one dependency that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1);
	}

	/// <summary>
	/// A job with two dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2);
	}

	/// <summary>
	/// A job with three dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3);
	}

	/// <summary>
	/// A job with four dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	}

	/// <summary>
	/// A job with five dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	}

	/// <summary>
	/// A job with six dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
	}

	/// <summary>
	/// A job with seven dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
	}

	/// <summary>
	/// A job with eight dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
	}

	/// <summary>
	/// A job with nine dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
	}

	/// <summary>
	/// A job with ten dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
	}

	/// <summary>
	/// A job with eleven dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T11">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <param name="arg11">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
	}

	/// <summary>
	/// A job with twelve dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T11">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T12">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <param name="arg11">A dependency required to process the item.</param>
		/// <param name="arg12">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
	}

	/// <summary>
	/// A job with thirteen dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T11">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T12">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T13">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <param name="arg11">A dependency required to process the item.</param>
		/// <param name="arg12">A dependency required to process the item.</param>
		/// <param name="arg13">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
	}

	/// <summary>
	/// A job with fourteen dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T11">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T12">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T13">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T14">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <param name="arg11">A dependency required to process the item.</param>
		/// <param name="arg12">A dependency required to process the item.</param>
		/// <param name="arg13">A dependency required to process the item.</param>
		/// <param name="arg14">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
	}

	/// <summary>
	/// A job with fifteen dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T11">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T12">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T13">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T14">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T15">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <param name="arg11">A dependency required to process the item.</param>
		/// <param name="arg12">A dependency required to process the item.</param>
		/// <param name="arg13">A dependency required to process the item.</param>
		/// <param name="arg14">A dependency required to process the item.</param>
		/// <param name="arg15">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
	}

	/// <summary>
	/// A job with sixteen dependencies that provides a list of items and an action to perform on each item.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T4">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T5">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T6">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T7">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T8">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T9">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T10">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T11">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T12">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T13">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T14">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T15">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T16">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : JobBase<TConfig, TItem>
	where TConfig : Config
	{
		/// <summary>
		/// Initializes a new Job.
		/// </summary>
		/// <param name="config">The config for the job.</param>
		protected Job(TConfig config)
			: base(config) { }

		/// <summary>
		/// Gets the <see cref="Execution"/> implementation to execute this job. You should not override this method.
		/// </summary>
		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this, this, provider);

		/// <summary>
		/// Processes the specified <paramref name="item"/>.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <param name="arg1">A dependency required to process the item.</param>
		/// <param name="arg2">A dependency required to process the item.</param>
		/// <param name="arg3">A dependency required to process the item.</param>
		/// <param name="arg4">A dependency required to process the item.</param>
		/// <param name="arg5">A dependency required to process the item.</param>
		/// <param name="arg6">A dependency required to process the item.</param>
		/// <param name="arg7">A dependency required to process the item.</param>
		/// <param name="arg8">A dependency required to process the item.</param>
		/// <param name="arg9">A dependency required to process the item.</param>
		/// <param name="arg10">A dependency required to process the item.</param>
		/// <param name="arg11">A dependency required to process the item.</param>
		/// <param name="arg12">A dependency required to process the item.</param>
		/// <param name="arg13">A dependency required to process the item.</param>
		/// <param name="arg14">A dependency required to process the item.</param>
		/// <param name="arg15">A dependency required to process the item.</param>
		/// <param name="arg16">A dependency required to process the item.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
	}
}
