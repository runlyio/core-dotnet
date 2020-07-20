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
			var args = "run DiagnosticJob --NumberOfItems 10 --Names will chad --JobMethod ProcessAsync --WaitForSignalInInitializeAsync --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(DiagnosticJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetRequiredService<DiagnosticConfig>();
			_ = provider.GetRequiredService<IHostAction>();

			config.NumberOfItems.Should().Be(10);
			config.Names.Should().BeEquivalentTo(new[] { "will", "chad" });
			config.JobMethod.Should().Be(JobMethod.ProcessAsync);
			config.WaitForSignalInInitializeAsync.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}

		[Fact]
		public void Should_apply_lowercase_overrides()
		{
			var args = "run DiagnosticJob --NumberOfItems 10 --WaitForSignalInInitializeAsync --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".ToLowerInvariant().Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(DiagnosticJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetRequiredService<DiagnosticConfig>();
			_ = provider.GetRequiredService<IHostAction>();

			config.NumberOfItems.Should().Be(10);
			config.WaitForSignalInInitializeAsync.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}

		[Fact]
		public void Should_fail_on_invalid_config_properties()
		{
			var args = "run DiagnosticJob --ZumberOfItems 10 --WaitForSignalInInitializeAsync --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".ToLowerInvariant().Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(DiagnosticJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetService<DiagnosticConfig>();
			var host = provider.GetService<IHostAction>();

			config.Should().BeNull();
			host.Should().BeNull();
		}
	}
}
