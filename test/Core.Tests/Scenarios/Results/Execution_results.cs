using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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

			testHost.Services.AddLogging();
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

			var run = testHost.CreateRun();
			await run.RunAsync();

			run.Execution.ItemCategories.Should().BeEquivalentTo(new []
			{
				new ItemCategory() { IsSuccessful = true, count = 5, Category = "Good" },
				new ItemCategory() { IsSuccessful = true, count = 4, Category = "Great" },
				new ItemCategory() { IsSuccessful = false, count = 3, Category = "Nah" },
				new ItemCategory() { IsSuccessful = false, count = 2, Category = "Bruh" },
			}, o => o.WithoutStrictOrdering());
		}
	}
}
