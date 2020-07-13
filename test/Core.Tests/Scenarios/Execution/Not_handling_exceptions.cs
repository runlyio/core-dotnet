using FluentAssertions;
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
		}

		[Fact]
		public async Task Should_not_handle_failure_in_initialize()
		{
			config.ThrowExceptionInInitializeAsync = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.InitializeAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_getitems()
		{
			config.ThrowExceptionInGetItemsAsync = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.GetItemsAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_get_enumerator()
		{
			config.ThrowExceptionInGetEnumerator = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.GetEnumerator);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_enumerator_movenext()
		{
			config.ThrowExceptionInEnumeratorMoveNext = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.EnumeratorMoveNext);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_enumerator_current()
		{
			config.CanCountItems = false;
			config.ThrowExceptionInEnumeratorCurrent = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.EnumeratorCurrent);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_item_getitemidasync()
		{
			config.ThrowExceptionInGetItemIdAsync = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.GetItemIdAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_process()
		{
			config.ThrowExceptionInProcessAsync = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.ProcessAsync);
		}

		[Fact]
		public async Task Should_not_handle_failure_in_finalize()
		{
			config.ThrowExceptionInFinalizeAsync = true;

			using var runner = testHost.BuildRunner();

			(await runner.Invoking(a => a.RunAsync())
				.Should().ThrowAsync<DiagnosticJobException>())
				.And.Location.Should().Be(JobMethod.FinalizeAsync);
		}
	}
}
