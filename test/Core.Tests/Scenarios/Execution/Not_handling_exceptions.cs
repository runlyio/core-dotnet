using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Runly.Diagnostics;
using Runly.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Execution
{
	public class Not_handling_exceptions
	{
		readonly TestHost<DiagnosticJob> testHost;
		readonly DiagnosticConfig config;

		public Not_handling_exceptions()
		{
			config = new DiagnosticConfig()
			{
				NumberOfItems = 1,
				CanCountItems = false
			};

			config.Execution.HandleExceptions = false;

			testHost = new TestHost<DiagnosticJob>(config);

			testHost.Services.AddLogging();
		}

		[Fact]
		public async Task Should_not_handle_failure_in_initialize()
		{
			config.ThrowExceptionInInitializeAsync = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.InitializeAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_getitems()
		{
			config.ThrowExceptionInGetItemsAsync = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.GetItemsAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_get_enumerator()
		{
			config.ThrowExceptionInGetEnumerator = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.GetEnumerator);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_enumerator_movenext()
		{
			config.ThrowExceptionInEnumeratorMoveNext = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.EnumeratorMoveNext);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_enumerator_current()
		{
			config.CanCountItems = false;
			config.ThrowExceptionInEnumeratorCurrent = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.EnumeratorCurrent);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_item_getitemidasync()
		{
			config.ThrowExceptionInGetItemIdAsync = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.GetItemIdAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_process()
		{
			config.ThrowExceptionInProcessAsync = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.ProcessAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_finalize()
		{
			config.ThrowExceptionInFinalizeAsync = true;

			(await testHost.BuildRunner().Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.FinalizeAsync);
		}
	}
}
