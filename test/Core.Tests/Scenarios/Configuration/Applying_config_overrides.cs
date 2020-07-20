using FluentAssertions;
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
			var config = new DiagnosticConfig();
			var reader = new ConfigReader(new JobCache(new[] { typeof(DiagnosticJob).Assembly }));
			
			reader.ApplyOverrides(config, new[]
			{
				"NumberOfItems=10",
				"WaitForSignalInInitializeAsync=true",
				"RunlyApi.Token=1234asdf",
				"Execution.RunAfterId=6B165086-E24D-49B5-A57D-57EBB080C0C1"
			});

			config.NumberOfItems.Should().Be(10);
			config.WaitForSignalInInitializeAsync.Should().BeTrue();
			config.RunlyApi.Token = "1234asdf";
			config.Execution.RunAfterId.Should().Be(Guid.Parse("6B165086-E24D-49B5-A57D-57EBB080C0C1"));
		}
	}
}
