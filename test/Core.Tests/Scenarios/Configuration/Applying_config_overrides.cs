using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Runly.Diagnostics;
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
			var args = "run DiagnosticJob --NumberOfItems 10 --WaitForSignalInInitializeAsync --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(DiagnosticJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetRequiredService<DiagnosticConfig>();

			config.NumberOfItems.Should().Be(10);
			config.WaitForSignalInInitializeAsync.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}

		[Fact]
		public void Should_apply_case_insensitive_overrides()
		{
			var args = "run DiagnosticJob --NumberOfItems 10 --WaitForSignalInInitializeAsync --RunlyApi.Token 1234asdf --Execution.RunAfterId 6B165086-E24D-49B5-A57D-57EBB080C0C1".ToLowerInvariant().Split(' ');

			var services = new ServiceCollection();
			services.AddRunlyJobs(args, typeof(DiagnosticJob).Assembly);

			var provider = services.BuildServiceProvider();

			var config = provider.GetRequiredService<DiagnosticConfig>();

			config.NumberOfItems.Should().Be(10);
			config.WaitForSignalInInitializeAsync.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}
	}
}
