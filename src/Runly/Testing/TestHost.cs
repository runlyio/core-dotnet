using Microsoft.Extensions.DependencyInjection;
using System;

namespace Runly.Testing
{
	public class TestHost<TJob>
		where TJob : IJob
	{
		readonly Config config;
		Action<IServiceCollection> configureDelegate;

		public TestHost(Config config)
		{
			this.config = config ?? throw new ArgumentNullException(nameof(config));

			if (string.IsNullOrWhiteSpace(config.Job.Type))
				config.Job.Type = typeof(TJob).FullName;
		}

		public TestHost<TJob> ConfigureServices(Action<IServiceCollection> configureDelegate)
		{
			this.configureDelegate = configureDelegate;
			return this;
		}

		/// <summary>
		/// Creates a <see cref="TestRun{TJob}"/> using the <see cref="ServicesCollection"/>
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
