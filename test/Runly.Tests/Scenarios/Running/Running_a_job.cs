using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Runly.Hosting;
using Runly.Testing;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Running
{
	public class Running_a_job
	{
		[Fact]
		public async Task should_run_a_no_config_job()
		{
			using var runner = new TestHost<JobNoConf>(new Config()).BuildRunner();

			await runner.RunAsync();

			runner.Execution.IsComplete.Should().BeTrue();
			runner.Execution.Disposition.Should().Be(Disposition.Successful);
		}

		[Fact]
		public async Task should_run_a_single_item_job()
		{
			using var runner = new TestHost<JobSingleItem>(new Config()).BuildRunner();

			await runner.RunAsync();

			runner.Execution.IsComplete.Should().BeTrue();
			runner.Execution.Disposition.Should().Be(Disposition.Successful);
		}

        [Fact]
        public async Task should_run_a_job_with_scoped_dependency_in_constructor()
        {
			// CreateDefaultBuilder is more strict with Environment = Dev
			Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

			var signal = new AutoResetEvent(false);

            var action = JobHost.CreateDefaultBuilder(["Job2WithConstructorDep"], typeof(UnitTest).Assembly)
                 .ConfigureServices((context, services) =>
                 {
                     services.AddScoped<IDep1>(s => new Dep1());
					 services.AddSingleton<IDep2>(s => new Dep2());
					 services.AddSingleton(signal);
                 })
                .Build();

			var runAction = action.Services.GetRequiredService<IHostedService>() as RunAction;

			((SignalConfig)runAction.Config).WaitForSignal = true;

			Dep1.IsDisposed.Should().BeFalse();
			
            var run = action.RunAsync();

            Dep1.IsDisposed.Should().BeFalse();

			signal.Set();

            await run;

			Dep1.IsDisposed.Should().BeTrue();
        }
    }
}
