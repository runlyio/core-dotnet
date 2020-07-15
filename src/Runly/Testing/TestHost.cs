using Microsoft.Extensions.DependencyInjection;
using System;

namespace Runly.Testing
{
	/// <summary>
	/// A program initialization abstraction for testing Runly jobs.
	/// </summary>
	/// <typeparam name="TJob">The type of <see cref="IJob"/> being tested.</typeparam>
	public class TestHost<TJob>
		where TJob : IJob
	{
		readonly Config config;
		Action<IServiceCollection> configureDelegate;

		/// <summary>
		/// Initializes the <see cref="TestHost{TJob}"/>.
		/// </summary>
		/// <param name="config">The config for the <see cref="IJob"/> to run.</param>
		public TestHost(Config config)
		{
			this.config = config ?? throw new ArgumentNullException(nameof(config));

			if (string.IsNullOrWhiteSpace(config.Job.Type))
				config.Job.Type = typeof(TJob).FullName;
		}

		/// <summary>
		/// Adds services to the container. This can be called multiple times and the results will be additive.
		/// </summary>
		/// <param name="configureDelegate">The delegate for configuring the <see cref="IServiceCollection"/> that will be used to construct the <see cref="IServiceProvider"/>.</param>
		/// <returns>The same instance of the <see cref="TestHost{TJob}"/> for chaining.</returns>
		public TestHost<TJob> ConfigureServices(Action<IServiceCollection> configureDelegate)
		{
			this.configureDelegate = configureDelegate;
			return this;
		}

		/// <summary>
		/// Creates a new <see cref="TestRun{TJob}"/>.
		/// </summary>
		public TestRun<TJob> BuildRunner()
		{
			var services = new ServiceCollection();
			services.AddRunlyJobs(config, typeof(TJob).Assembly);
			services.AddLogging();

			if (configureDelegate != null)
				configureDelegate(services);

			var provider = services.BuildServiceProvider();
			var execution = provider.GetRequiredService<Execution>();
			return new TestRun<TJob>((TJob)execution.Job, execution, new ResultLog(execution), provider);
		}
	}
}
