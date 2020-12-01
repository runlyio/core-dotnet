using FluentAssertions;
using Runly.Testing;
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
	}
}
