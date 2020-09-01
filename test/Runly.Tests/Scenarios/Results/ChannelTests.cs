using FluentAssertions;
using Runly.Client;
using Runly.Client.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Results
{
	public class ChannelTests : IAsyncDisposable
	{
		readonly FakeResultsConnection connection;
		readonly ResultsChannel.Connection channel;
		readonly int minDelay, maxDelay;

		public ChannelTests()
		{
			connection = new FakeResultsConnection();
			channel = new ResultsChannel.Connection(connection, new FakeLogger<ResultsChannel>());

			minDelay = ResultsChannel.Connection.MinDelay + 200;
			maxDelay = ResultsChannel.Connection.MaxDelay + 200;
		}

		public async ValueTask DisposeAsync()
		{
			if (connection != null)
				await connection.DisposeAsync();

			if (channel != null)
				await channel.DisposeAsync();
		}

		[Fact]
		public async Task Should_queue_messages_when_trying_to_reconnect()
		{
			await connection.MarkReconnecting();

			await channel.UpdateState(InstanceState.Processing);
			await Task.Delay(minDelay);

			connection.InvokedAndSentMethods.Should().BeEmpty();

			await connection.MarkConnected(isReconnected: true);
			await Task.Delay(minDelay);

			connection.InvokedAndSentMethods.Should().HaveCount(1);

			var method = connection.InvokedAndSentMethods.Single();
			method.Should().BeEquivalentTo(new HubMethodCall(nameof(channel.UpdateState), InstanceState.Processing, new ItemProgress[0]));
		}

		[Fact]
		public async Task Should_try_to_reconnect_after_full_disconnection()
		{
			await connection.MarkClosed(isNormal: false);

			await channel.UpdateState(InstanceState.Processing);
			await Task.Delay(minDelay);

			connection.InvokedAndSentMethods.Should().BeEmpty();
			connection.StartAttempts.Should().Be(1);
		}

		[Fact]
		public async Task Should_not_try_to_reconnect_after_normal_full_disconnection()
		{
			await connection.MarkClosed(isNormal: true);
			await Task.Delay(minDelay);
			connection.StartAttempts.Should().Be(0);
		}

		[Fact]
		public async Task Should_send_multiple_messages_in_order()
		{
			await channel.SetTotal(42);
			await channel.UpdateState(InstanceState.GettingItemsToProcess);

			await Task.Delay(maxDelay);

			connection.InvokedAndSentMethods.Should().HaveCount(2);

			connection.InvokedAndSentMethods.Should().BeEquivalentTo(new[]
			{
				new HubMethodCall(nameof(channel.SetTotal), 42),
				new HubMethodCall(nameof(channel.UpdateState), InstanceState.GettingItemsToProcess, new ItemProgress[0]),
			}, o => o.WithStrictOrdering());
		}

		[Fact]
		public async Task Should_not_send_messages_out_of_order_on_exception()
		{
			connection.ThrowOnSend = true;
			connection.DisconnectOnSendError = true;

			await channel.SetTotal(42);
			await channel.UpdateState(InstanceState.GettingItemsToProcess);
			await Task.Delay(maxDelay);

			connection.InvokedAndSentMethods.Should().BeEmpty();

			connection.ThrowOnSend = false;
			await connection.MarkConnected(isReconnected: true);

			await channel.UpdateState(InstanceState.Finalizing);

			await Task.Delay(maxDelay);

			connection.InvokedAndSentMethods.Should().HaveCount(3);

			connection.InvokedAndSentMethods.Should().BeEquivalentTo(new[]
			{
				new HubMethodCall(nameof(channel.SetTotal), 42),
				new HubMethodCall(nameof(channel.UpdateState), InstanceState.GettingItemsToProcess, new ItemProgress[0]),
				new HubMethodCall(nameof(channel.UpdateState), InstanceState.Finalizing, new ItemProgress[0])
			}, o => o.WithStrictOrdering());
		}

		[Fact]
		public async Task Should_discard_poison_messages()
		{
			connection.ThrowOnSend = true;

			await channel.SetTotal(42);
			await channel.FlushAsync();

			connection.ThrowOnSend = false;

			await channel.UpdateState(InstanceState.GettingItemsToProcess);
			await channel.FlushAsync();

			connection.InvokedAndSentMethods.Should().HaveCount(1);

			connection.InvokedAndSentMethods.Should().BeEquivalentTo(new[]
			{
				new HubMethodCall(nameof(channel.UpdateState), InstanceState.GettingItemsToProcess, new ItemProgress[0]),
			});
		}
	}
}
