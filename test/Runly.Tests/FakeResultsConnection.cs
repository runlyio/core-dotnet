using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Runly.Client;

namespace Runly.Tests
{
	public class FakeResultsConnection : IResultsConnection
	{
		public event Func<Exception, Task> Closed;
		public event Func<string, Task> Reconnected;
		public event Func<Exception, Task> Reconnecting;

		public int StartAttempts { get; private set; }
		public HubConnectionState State { get; private set; } = HubConnectionState.Connected;

		public List<HubMethodCall> InvokedAndSentMethods { get; } = new List<HubMethodCall>();

		public bool ThrowOnSend { get; set; }

		public bool DisconnectOnSendError { get; set; }

		Task closeTask;

		public async Task MarkReconnecting()
		{
			State = HubConnectionState.Reconnecting;
			if (Reconnecting != null)
				await Reconnecting(new Exception());
		}

		public async Task MarkClosed(bool isNormal)
		{
			State = HubConnectionState.Disconnected;

			if (Closed != null)
				await Closed(isNormal ? null : new Exception());
		}

		public async Task MarkConnected(bool isReconnected)
		{
			State = HubConnectionState.Connected;

			if (isReconnected && Reconnected != null)
				await Reconnected("1234");
		}

		public Task StartAsync()
		{
			StartAttempts++;
			return Task.CompletedTask;
		}

		public async ValueTask DisposeAsync()
		{
			if (closeTask != null)
				await closeTask;
		}

		public Task InvokeCoreAsync(string methodName, object[] args)
		{
			if (ThrowOnSend)
			{
				// simulate a disconnection
				if (DisconnectOnSendError)
					closeTask = MarkClosed(false);

				throw new Exception("let's pretend an error happened while trying to send a message");
			}

			InvokedAndSentMethods.Add(new HubMethodCall(methodName, args));
			return Task.CompletedTask;
		}

		public Task SendCoreAsync(string methodName, object[] args)
		{
			if (ThrowOnSend)
			{
				// simulate a disconnection
				if (DisconnectOnSendError)
					closeTask = MarkClosed(false);

				throw new Exception("let's pretend an error happened while trying to send a message");
			}

			InvokedAndSentMethods.Add(new HubMethodCall(methodName, args));
			return Task.CompletedTask;
		}

		public IDisposable On(string methodName, Action handler) => new FakeDisposer();
		class FakeDisposer : IDisposable { public void Dispose() { } }
	}
}
