using FluentAssertions;
using Runly.Diagnostics;
using Runly.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Results
{
	public class Execution_results
	{
		readonly TestHost<DiagnosticJob> testHost;
		readonly DiagnosticConfig config;

		public Execution_results()
		{
			config = new DiagnosticConfig()
			{
				CanCountItems = true,
				NumberOfItems = 2
			};

			testHost = new TestHost<DiagnosticJob>(config);
		}

		[Fact]
		public async Task Should_sum_categories()
		{
			config.Execution.ItemFailureCountToStopJob = 10;
			config.Categories = new DiagnosticConfig.Category[]
			{
				new DiagnosticConfig.Category() { IsSuccessful = true, Count = 5, Name = "Good" },
				new DiagnosticConfig.Category() { IsSuccessful = true, Count = 4, Name = "Great" },
				new DiagnosticConfig.Category() { IsSuccessful = false, Count = 3, Name = "Nah" },
				new DiagnosticConfig.Category() { IsSuccessful = false, Count = 2, Name = "Bruh" }
			};

			using var run = testHost.BuildRunner();
			await run.RunAsync();

			run.Execution.ItemCategories.Should().BeEquivalentTo(new[]
			{
				new ItemCategory(5, true, "Good"),
				new ItemCategory(4, true, "Great"),
				new ItemCategory(3, false, "Nah"),
				new ItemCategory(2, false, "Bruh")
			}, o => o.WithoutStrictOrdering());
		}
	}
}
