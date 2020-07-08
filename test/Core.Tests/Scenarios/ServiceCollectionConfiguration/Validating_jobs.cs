using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Runly.Diagnostics;
using System.Linq;
using Xunit;

namespace Runly.Tests.Scenarios.ServiceCollectionConfiguration
{
	public class Validating_jobs
	{
		ServiceProvider BuildServiceProvider()
		{
			var services = new ServiceCollection();
			services.AddRunlyJobs(new[] { "list" }, typeof(Validating_jobs).Assembly, typeof(DiagnosticJob).Assembly);
			return services.BuildServiceProvider();
		}

		[Fact]
		public void Should_catch_invalid_jobs()
		{
			using (var sp = BuildServiceProvider())
			{
				var pc = sp.GetRequiredService<JobCache>();

				var info = pc.Jobs.Single(i => i.JobType == typeof(AbstractJob));

				info.IsValid.Should().BeFalse();
				info.Errors.Should().Be(JobLoadErrors.IsAbstract);

				info = pc.Jobs.Single(i => i.JobType == typeof(GenericJob<>));

				info.IsValid.Should().BeFalse();
				info.Errors.Should().Be(JobLoadErrors.IsGenericTypeDefinition);

				info = pc.Jobs.Single(i => i.JobType == typeof(DiagnosticJob));

				info.IsValid.Should().BeTrue();
				info.Errors.Should().Be(JobLoadErrors.None);
			}
		}
	}
}