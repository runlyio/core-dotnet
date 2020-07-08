using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Testing
{
	public class TestHost<TJob>
		where TJob : IJob
	{
		public IServiceCollection Services { get; }

		public TestHost(Config config)
			: this(config, new ServiceCollection()) { }

		public TestHost(Config config, IServiceCollection services)
		{
			_ = config ?? throw new ArgumentNullException(nameof(config));
			this.Services = services ?? throw new ArgumentNullException(nameof(services));

			if (string.IsNullOrWhiteSpace(config.Job.Type))
				config.Job.Type = typeof(TJob).FullName;

			services.AddRunlyJobs(config, typeof(TJob).Assembly);
		}
		
		/// <summary>
		/// Creates a <see cref="TestRun{TJob}"/> using the <see cref="ServicesCollection"/>
		/// </summary>
		public TestRun<TJob> CreateRun()
		{ 
			var execution = Services.BuildServiceProvider().GetRequiredService<Execution>();

			return new TestRun<TJob>((TJob)execution.Job, execution, new ResultLog(execution));
		}
	}
}
