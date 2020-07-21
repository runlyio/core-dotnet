using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Runly.Diagnostics;
using Runly.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Runly.Tests.Scenarios.Configuration
{
	public class Applying_config_overrides
	{
		[Fact]
		public void Should_apply_overrides()
		{
			var args = "CliTestJob --Int32 10 --StringArray will chad --Enum ProcessAsync --IsBool --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(CliTestJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetRequiredService<CliTestJobConfig>();
			provider.GetRequiredService<IHostAction>().Should().BeOfType<RunAction>();

			config.Int32.Should().Be(10);
			config.StringArray.Should().BeEquivalentTo(new[] { "will", "chad" });
			config.Enum.Should().Be(JobMethod.ProcessAsync);
			config.IsBool.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}

		[Fact]
		public void Should_apply_lowercase_overrides()
		{
			var args = "CliTestJob --Int32 10 --IsBool --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".ToLowerInvariant().Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(CliTestJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetRequiredService<CliTestJobConfig>();
			provider.GetRequiredService<IHostAction>().Should().BeOfType<RunAction>();

			config.Int32.Should().Be(10);
			config.IsBool.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}

		[Fact]
		public void Should_fail_on_invalid_config_properties()
		{
			var args = "CliTestJob --NumberOfItems 10 --isbool --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".ToLowerInvariant().Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(CliTestJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetService<CliTestJobConfig>();
			var host = provider.GetService<IHostAction>();

			config.Should().BeNull();
			host.Should().BeNull();
		}
	}
}
