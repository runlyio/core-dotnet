using Runly.Processing;
using System;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// A configurationless, single-action <see cref="Task"/>-based asynchronous job.
	/// </summary>
	public abstract class Job : Job<Config>
	{
		protected Job(Config config)
			: base(config) { }
	}

	/// <summary>
	/// A single-action <see cref="Task"/>-based asynchronous job.
	/// </summary>
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	public abstract class Job<TConfig> : JobBase<TConfig>
		where TConfig : Config
	{
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig>(this, provider);

		public abstract Task<Result> ProcessAsync();
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job.
	/// </summary>
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	public abstract class Job<TConfig, TItem> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with one dependency.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with two dependencies.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with three dependencies.
	/// </summary>	
	/// <typeparam name="TConfig">The configuration for the job.</typeparam>
	/// <typeparam name="TItem">The data required to process each item.</typeparam>
	/// <typeparam name="T1">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T2">A dependency passed into the ProcessAsync method.</typeparam>
	/// <typeparam name="T3">A dependency passed into the ProcessAsync method.</typeparam>
	public abstract class Job<TConfig, TItem, T1, T2, T3> : JobBase<TConfig, TItem>
		where TConfig : Config
	{
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with four dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with five dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with six dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with seven dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with eight dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with nine dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with ten dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with eleven dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with twelve dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with thirteen dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with fourteen dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with fifteen dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
	}

	/// <summary>
	/// A <see cref="Task"/>-based asynchronous job with sixteen dependencies.
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
		protected Job(TConfig config)
			: base(config) { }

		protected internal override Execution GetExecution(IServiceProvider provider) => new Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this, this, provider);

		public abstract Task<Result> ProcessAsync(TItem item, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
	}
}
