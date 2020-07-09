using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runly.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	public class ResultsChannel
	{
		readonly Uri baseUri;
		readonly IAuthenticationProvider auth;
		readonly ILogger<ResultsChannel> logger;
		readonly Action<HttpConnectionOptions> opts;

		/// <summary>
		/// When true, every message sent to the server will require a confirmation when 
		/// it has been processed. Enabling this feature will create a performance hit but
		/// eliminate race conditions that may occur when making unit test assertions.
		/// </summary>
		public bool RequireSendConfirmation { get; set; }

		public ResultsChannel(Uri baseUri, IAuthenticationProvider auth, ILogger<ResultsChannel> logger)
			: this(baseUri, auth, logger, null) { }

		public ResultsChannel(Uri baseUri, IAuthenticationProvider auth, ILogger<ResultsChannel> logger, Action<HttpConnectionOptions> opts)
		{
			this.baseUri = baseUri;
			this.auth = auth;
			this.logger = logger;
			this.opts = opts;
		}

		public async Task<Connection> ConnectAsync(Guid instanceId, CancellationToken token = default(CancellationToken))
		{
			var connection = new HubConnectionBuilder()
				.WithUrl(new Uri(baseUri, $"/nodes/results/{instanceId}"), o =>
				{
					o.AccessTokenProvider = auth.AcquireToken;

					if (opts != null)
						opts(o);
				})
				.WithAutomaticReconnect()
				.AddMessagePackProtocol()
				.Build();

			var channel = new Connection(new HubConnectionWrapper(connection), logger) { RequireSendConfirmation = this.RequireSendConfirmation };
			await connection.StartAsync(token);

			return channel;
		}

		/// <remarks>
		/// The underlying HubConnection is not thread-safe.
		/// We need to synchronize access to InvokeCoreAsync
		/// </remarks>
		public class Connection : IAsyncDisposable
		{
			public const int MinDelay = 100, MaxDelay = MinDelay * 5;

			readonly IResultsConnection conn;
			readonly ILogger<ResultsChannel> logger;

			public event Func<Task> CancellationRequested;

			readonly ConcurrentBag<Task> flushes = new ConcurrentBag<Task>();
			readonly SemaphoreSlim flushing = new SemaphoreSlim(1);
			readonly CancellationTokenSource flushCancellation = new CancellationTokenSource();
			readonly ConcurrentQueue<HubMethodCall> standard = new ConcurrentQueue<HubMethodCall>();
			readonly ConcurrentQueue<HubMethodCall> priority = new ConcurrentQueue<HubMethodCall>();

			/// <summary>
			/// When true, every message sent to the server will require a confirmation when 
			/// it has been processed. Enabling this feature will create a performance hit but
			/// eliminate race conditions that may occur when making unit test assertions.
			/// </summary>
			public bool RequireSendConfirmation { get; set; }

			public Connection(IResultsConnection conn, ILogger<ResultsChannel> logger)
			{
				this.conn = conn ?? throw new ArgumentNullException(nameof(conn));
				this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

				conn.Reconnecting += error =>
				{
					logger.LogWarning(error, "Temporarily disconnected from server; reconnecting");
					return Task.CompletedTask;
				};

				conn.Reconnected += connectionId =>
				{
					logger.LogInformation("And we're back! Sending results to server again.");
					return Task.CompletedTask;
				};

				conn.Closed += async error =>
				{
					if (error != null)
					{
						int delay = new Random().Next(0, 5);
						logger.LogWarning(error, "Lost connection or failed to reconnect to server. Will try connecting again in {delay} seconds.", delay);
						await Task.Delay(delay * 1000);

						await conn.StartAsync();
					}
				};

				conn.On("CancellationRequested", async () =>
				{
					if (CancellationRequested != null)
						await CancellationRequested();
				});
			}

			/// <summary>
			/// Blocks the caller until all messages in the queue have been sent
			/// </summary>
			public async Task FlushAsync()
			{
				while (flushes.TryTake(out Task flush))
					await flush;
			}

			async Task Flush()
			{
				try
				{
					await flushing.WaitAsync();

					if (flushCancellation.IsCancellationRequested)
						return;

					HubMethodCall priorityMethod = null, standardMethod = null;

					while (priority.TryPeek(out priorityMethod) || standard.TryPeek(out standardMethod))
					{
						while (conn.State != HubConnectionState.Connected && !flushCancellation.IsCancellationRequested)
							await Task.Delay(MinDelay);

						if (flushCancellation.IsCancellationRequested)
							break;

						bool dequeued = false;
						bool isPriority = priorityMethod != null;
						var method = priorityMethod ?? standardMethod;

						try
						{
							method.Attempts++;
							if (!RequireSendConfirmation && (method.MethodName == nameof(ItemResult) || method.MethodName == nameof(Log)))
								await conn.SendCoreAsync(method.MethodName, method.Args);
							else
								await conn.InvokeCoreAsync(method.MethodName, method.Args);

							dequeued = isPriority ? priority.TryDequeue(out _) : standard.TryDequeue(out _);
						}
						catch (Exception ex)
						{
							if (method.Attempts > 3)
							{
								logger.LogError(ex, $"Message discarded after 3 attempts: {method.MethodName}");

								if (!dequeued)
									_ = isPriority ? priority.TryDequeue(out _) : standard.TryDequeue(out _);
							}
							else
							{
								int retryDelay = new Random().Next(0, 5) * MinDelay;
								logger.LogWarning(ex, "Could not send {method} message to server. Will retry in {retryDelay}ms.", method.MethodName, retryDelay);
								await Task.Delay(retryDelay);
							}
						}

						priorityMethod = null;
						standardMethod = null;
					}
				}
				finally
				{
					flushing.Release();
				}
			}

			Task SendAsync(string methodName, params object[] args) => SendAsync(new HubMethodCall(methodName, args));

			async Task SendAsync(HubMethodCall msg)
			{
				if (RequireSendConfirmation)
				{
					await conn.InvokeCoreAsync(msg.MethodName, msg.Args);
				}
				else
				{
					standard.Enqueue(msg);
					flushes.Add(Flush());
				}
			}

			Task SendPriorityAsync(string methodName, params object[] args) => SendPriorityAsync(new HubMethodCall(methodName, args));

			async Task SendPriorityAsync(HubMethodCall msg)
			{
				if (RequireSendConfirmation)
				{
					await conn.InvokeCoreAsync(msg.MethodName, msg.Args);
				}
				else
				{
					priority.Enqueue(msg);
					flushes.Add(Flush());
				}
			}

			public Task StartApp(int pid) => SendPriorityAsync(nameof(StartApp), pid);

			public Task Abort() => SendPriorityAsync(nameof(Abort));

			public Task ExitApp(int exitCode, bool timedOut, long duration) =>
				SendPriorityAsync(nameof(ExitApp), exitCode, timedOut, duration);

			public Task FailApp(object details) => SendPriorityAsync(nameof(FailApp), details);

			public Task SetTotal(int total) => SendPriorityAsync(nameof(SetTotal), total);

			public Task UpdateState(InstanceState state, params ItemProgress[] categories) =>
				SendPriorityAsync(nameof(UpdateState), state, (IEnumerable<ItemProgress>)categories);

			public Task UpdateState(InstanceState state, IEnumerable<ItemProgress> categories) =>
				SendPriorityAsync(nameof(UpdateState), state, categories.ToArray());

			public Task MethodResult(MethodOutcome outcome) => SendPriorityAsync(nameof(MethodResult), outcome);

			public Task ItemResult(ItemResult result) => SendAsync(nameof(ItemResult), result);

			public Task Log(RunLogType type, int index, string log) => SendAsync(nameof(Log), type, index, log);

			public Task MarkComplete(Disposition disposition)
				=> MarkComplete(disposition, null);

			public Task MarkComplete(Disposition disposition, object output, params ItemProgress[] categories)
				=> MarkComplete(disposition, output, (IEnumerable<ItemProgress>)categories);

			public Task MarkComplete(Disposition disposition, object output, IEnumerable<ItemProgress> categories) =>
				SendPriorityAsync(nameof(MarkComplete), disposition, output, categories.ToArray());

			public async ValueTask DisposeAsync()
			{
				if (flushCancellation != null)
					flushCancellation.Cancel();

				await FlushAsync();

				if (conn != null)
					await conn.DisposeAsync();
			}
		}
	}

	public class HubMethodCall
	{
		public int Attempts { get; set; }
		public string MethodName { get; }
		public object[] Args { get; }

		public HubMethodCall(string methodName, params object[] args)
		{
			this.MethodName = methodName;
			this.Args = args;
		}

		public override string ToString() => MethodName;
	}

	public interface IResultsConnection : IAsyncDisposable
	{
		event Func<Exception, Task> Closed;
		event Func<string, Task> Reconnected;
		event Func<Exception, Task> Reconnecting;

		HubConnectionState State { get; }

		Task StartAsync();
		Task InvokeCoreAsync(string method, object[] args);
		Task SendCoreAsync(string method, object[] args);
		IDisposable On(string methodName, Action handler);
	}

	public class HubConnectionWrapper : IResultsConnection
	{
		readonly HubConnection connection;

		public HubConnectionWrapper(HubConnection connection)
		{
			this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		public event Func<Exception, Task> Closed
		{
			add { connection.Closed += value; }
			remove { connection.Closed -= value; }
		}

		public event Func<string, Task> Reconnected
		{
			add { connection.Reconnected += value; }
			remove { connection.Reconnected -= value; }
		}

		public event Func<Exception, Task> Reconnecting
		{
			add { connection.Reconnecting += value; }
			remove { connection.Reconnecting -= value; }
		}

		public HubConnectionState State => connection.State;

		public async ValueTask DisposeAsync()
		{
			if (connection != null)
				await connection.DisposeAsync();
		}

		public Task StartAsync() => connection.StartAsync();
		public Task InvokeCoreAsync(string methodName, object[] args) => connection.InvokeCoreAsync(methodName, args);
		public Task SendCoreAsync(string methodName, object[] args) => connection.SendCoreAsync(methodName, args);
		public IDisposable On(string methodName, Action handler) => connection.On(methodName, handler);
	}
}
