using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Runly.Diagnostics;
using Runly.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Cancel
{
	public class Cancelling_a_job
	{
		readonly TestHost<DiagnosticJob> testHost;
		readonly DiagnosticConfig config;

		public Cancelling_a_job()
		{
			config = new DiagnosticConfig()
			{
				CanCountItems = true,
				NumberOfItems = 2
			};

			testHost = new TestHost<DiagnosticJob>(config);

			testHost.Services.AddLogging();
		}

		[Fact]
		public async Task Should_cancel_running_job()
		{
			// Hold the job on the first item in ProcessAsync until Signal is called
			config.WaitForSignalInProcessAsync = true;

			var run = testHost.CreateRun();
			var running = run.RunAsync();

			run.Cancel();

			// Don't wait after the first call
			config.WaitForSignalInProcessAsync = false;
			run.Job.Signal();

			await running;

			run.Execution.IsCanceled.Should().BeTrue();
			run.Execution.CanceledAt.Should().NotBeNull();
			run.Execution.Disposition.Should().Be(Disposition.Cancelled);
			run.Execution.SuccessfulItemCount.Should().BeLessThan(run.Execution.TotalItemCount.Value);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public async Task Should_cancel_a_cancelled_job()
		{
			config.WaitForSignalInProcessAsync = true;

			var run = testHost.CreateRun();
			var running = run.RunAsync();

			run.Cancel();

			// Should do nothing
			run.Cancel();

			config.WaitForSignalInProcessAsync = false;
			run.Job.Signal();

			await running;

			run.Execution.IsCanceled.Should().BeTrue();
			run.Execution.CanceledAt.Should().NotBeNull();
			run.Execution.Disposition.Should().Be(Disposition.Cancelled);
			run.Execution.SuccessfulItemCount.Should().BeLessThan(run.Execution.TotalItemCount.Value);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public async Task Should_cancel_a_failed_job()
		{
			config.ThrowExceptionInInitializeAsync = true;

			var run = testHost.CreateRun();
			await run.RunAsync();

			// Should do nothing
			run.Cancel();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.IsCanceled.Should().BeFalse();
			run.Execution.CanceledAt.Should().BeNull();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public async Task Should_cancel_a_completed_job()
		{
			config.NumberOfItems = 1;
			config.MillisecondDelayPerItem = 10;

			var run = testHost.CreateRun();
			await run.RunAsync();

			// Should do nothing
			run.Cancel();

			run.Execution.IsFailed.Should().BeFalse();
			run.Execution.IsCanceled.Should().BeFalse();
			run.Execution.CanceledAt.Should().BeNull();
			run.Execution.Disposition.Should().Be(Disposition.Successful);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}
	}
}
