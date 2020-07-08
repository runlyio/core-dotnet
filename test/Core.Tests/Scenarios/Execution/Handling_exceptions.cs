using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Runly.Diagnostics;
using Runly.Testing;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Execution
{
	public class Handling_exceptions
	{
		readonly TestHost<TestJob> testHost;
		readonly TestConfig config;

		public Handling_exceptions()
		{
			config = new TestConfig()
			{
				NumberOfItems = 1,
				CanCountItems = false
			};

			testHost = new TestHost<TestJob>(config);

			testHost.Services.AddLogging();
		}

		[Fact]
		public async Task Should_handle_failure_in_initialize()
		{
			config.ThrowExceptionInInitializeAsync = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().BeNull();
			run.Job.ProcessedAt.Should().BeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeFalse();
			run.Results.InitializeAsync.Exception.Should().NotBeNull();
			run.Results.GetItemsAsync.Should().BeNull();
			run.Results.Items.Count().Should().Be(0);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failure_in_getitems()
		{
			config.ThrowExceptionInGetItemsAsync = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().BeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeFalse();
			run.Results.GetItemsAsync.Exception.Should().NotBeNull();
			run.Results.Items.Count().Should().Be(0);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failure_in_get_enumerator()
		{
			config.ThrowExceptionInGetEnumerator = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().BeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetEnumerator.IsSuccessful.Should().BeFalse();
			run.Results.GetEnumerator.Exception.Should().NotBeNull();
			run.Results.Items.Count().Should().Be(0);
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failure_in_enumerator_movenext()
		{
			config.ThrowExceptionInEnumeratorMoveNext = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().BeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetEnumerator.IsSuccessful.Should().BeTrue();
			run.Results.Items.Count().Should().Be(1);
			run.Results.Items.Single().EnumeratorMoveNext.Should().NotBeNull();
			run.Results.Items.Single().EnumeratorMoveNext.IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().EnumeratorMoveNext.Exception.Should().NotBeNull();
			run.Results.Items.Single().EnumeratorCurrent.Should().BeNull();
			run.Results.Items.Single().ProcessAsync.Should().BeNull();
			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().Category.Should().NotBeNullOrEmpty();
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();

			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().FailedDueToException.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failure_in_enumerator_current()
		{
			config.ThrowExceptionInEnumeratorCurrent = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().BeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetEnumerator.IsSuccessful.Should().BeTrue();
			run.Results.Items.Count().Should().Be(1);
			run.Results.Items.Single().EnumeratorMoveNext.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().EnumeratorCurrent.Should().NotBeNull();
			run.Results.Items.Single().EnumeratorCurrent.IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().EnumeratorCurrent.Exception.Should().NotBeNull();
			run.Results.Items.Single().ProcessAsync.Should().BeNull();
			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().Category.Should().NotBeNullOrEmpty();
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();

			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().FailedDueToException.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failure_in_getitemidasync()
		{
			config.ThrowExceptionInGetItemIdAsync = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().BeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetEnumerator.IsSuccessful.Should().BeTrue();
			run.Results.Items.Count().Should().Be(1);
			run.Results.Items.Single().EnumeratorMoveNext.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().EnumeratorCurrent.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().GetItemIdAsync.Should().NotBeNull();
			run.Results.Items.Single().GetItemIdAsync.IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().GetItemIdAsync.Exception.Should().NotBeNull();
			run.Results.Items.Single().ProcessAsync.Should().BeNull();
			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().Category.Should().NotBeNullOrEmpty();
			run.Results.Items.Single().Id.Should().Be("Unknown");
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();

			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().FailedDueToException.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failure_in_process()
		{
			config.ThrowExceptionInProcessAsync = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().NotBeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Count().Should().Be(1);
			run.Results.Items.Single().EnumeratorMoveNext.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().EnumeratorCurrent.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().GetItemIdAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().ProcessAsync.Should().NotBeNull();
			run.Results.Items.Single().ProcessAsync.IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().ProcessAsync.Exception.Should().NotBeNull();
			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().Category.Should().NotBeNullOrEmpty();
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();

			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().FailedDueToException.Should().BeTrue();
		}

		[Fact]
		public async Task Should_handle_failed_result_in_process()
		{
			config.Categories = new TestConfig.Category[]
			{
				new TestConfig.Category
				{
					Count = 1,
					IsSuccessful = false
				}
			};

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().NotBeNull();
			run.Job.FinalizedAt.Should().NotBeNull();
			
			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Count().Should().Be(1);
			run.Results.Items.Single().EnumeratorMoveNext.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().EnumeratorCurrent.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().GetItemIdAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().ProcessAsync.Should().NotBeNull();
			run.Results.Items.Single().ProcessAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().ProcessAsync.Exception.Should().BeNull();
			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().Category.Should().NotBeNullOrEmpty();
			run.Results.FinalizeAsync.IsSuccessful.Should().BeTrue();

			run.Results.Items.Single().IsSuccessful.Should().BeFalse();
			run.Results.Items.Single().FailedDueToException.Should().BeFalse();
		}

		[Fact]
		public async Task Should_handle_failure_in_finalize()
		{
			config.ThrowExceptionInFinalizeAsync = true;

			var run = await testHost.CreateRun().RunAsync();

			run.Job.InitializedAt.Should().NotBeNull();
			run.Job.ItemsRetrievedAt.Should().NotBeNull();
			run.Job.ProcessedAt.Should().NotBeNull();
			run.Job.FinalizedAt.Should().NotBeNull();

			run.Execution.IsFailed.Should().BeTrue();
			run.Execution.Disposition.Should().Be(Disposition.Failed);
			run.Results.InitializeAsync.IsSuccessful.Should().BeTrue();
			run.Results.GetItemsAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Count().Should().Be(1);
			run.Results.Items.Single().EnumeratorMoveNext.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().EnumeratorCurrent.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().GetItemIdAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().ProcessAsync.IsSuccessful.Should().BeTrue();
			run.Results.Items.Single().IsSuccessful.Should().BeTrue();
			run.Results.FinalizeAsync.IsSuccessful.Should().BeFalse();
			run.Results.FinalizeAsync.Exception.Should().NotBeNull();
		}
	}
}
