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
	/// <summary>
	/// A SignalR channel for communicating job results to the Runly API.
	/// </summary>
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

		/// <summary>
		/// Initializes a new <see cref="ResultsChannel"/>.
		/// </summary>
		/// <param name="baseUri">The <see cref="Uri"/> to the results hub.</param>
		/// <param name="auth">The <see cref="IAuthenticationProvider"/> to use.</param>
		/// <param name="logger">A <see cref="ILogger"/> to use.</param>
		public ResultsChannel(Uri baseUri, IAuthenticationProvider auth, ILogger<ResultsChannel> logger)
			: this(baseUri, auth, logger, null) { }

		/// <summary>
		/// Initializes a new <see cref="ResultsChannel"/>.
		/// </summary>
		/// <param name="baseUri">The <see cref="Uri"/> to the results hub.</param>
		/// <param name="auth">The <see cref="IAuthenticationProvider"/> to use.</param>
		/// <param name="logger">A <see cref="ILogger"/> to use.</param>
		/// <param name="opts">Action to modify <see cref="HttpConnectionOptions"/>.</param>
		public ResultsChannel(Uri baseUri, IAuthenticationProvider auth, ILogger<ResultsChannel> logger, Action<HttpConnectionOptions> opts)
		{
			this.baseUri = baseUri;
			this.auth = auth;
			this.logger = logger;
			this.opts = opts;
		}

		/// <summary>
		/// Creates a connection to the results hub.
		/// </summary>
		/// <param name="instanceId">The run's instance ID.</param>
		/// <param name="token">The token to trigger cancellation.</param>
		/// <returns>A <see cref="Connection"/> to the results hub.</returns>
		public async Task<Connection> ConnectAsync(Guid instanceId, CancellationToken token = default(CancellationToken))
		{
			var connection = new HubConnectionBuilder()
				.WithUrl(new Uri(baseUri, $"/nodes/results/{instanceId}"), o =>
				{
					o.AccessTokenProvider = auth.AcquireToken;

					opts?.Invoke(o);
				})
				.WithAutomaticReconnect()
				.AddMessagePackProtocol()
				.Build();

			var channel = new Connection(new HubConnectionWrapper(connection), logger) { RequireSendConfirmation = this.RequireSendConfirmation };
			await connection.StartAsync(token);

			return channel;
		}

		/// <summary>
		/// Connection to the results hub.
		/// </summary>
		public class Connection : IAsyncDisposable
		{
			internal const int MinDelay = 100, MaxDelay = MinDelay * 5;

			readonly IResultsConnection conn;
			readonly ILogger<ResultsChannel> logger;

			/// <summary>
			/// Event raises when cancellation is triggered.
			/// </summary>
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

			/// <summary>
			/// Initializes a new <see cref="Connection"/>.
			/// </summary>
			/// <param name="conn"></param>
			/// <param name="logger"></param>
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

			/// <summary>
			/// Call made when the node observes the job app starting.
			/// </summary>
			/// <param name="pid">The local process ID of the job app.</param>
			public Task StartApp(int pid) => SendPriorityAsync(nameof(StartApp), pid);

			/// <summary>
			/// Call made when the node aborts the starting of a job because of a user-requested cancellation during acquisition.
			/// </summary>
			/// <returns></returns>
			public Task Abort() => SendPriorityAsync(nameof(Abort));

			/// <summary>
			/// Call made when the node observes the job app exiting.
			/// </summary>
			/// <param name="exitCode">The exit code of the job app process.</param>
			/// <param name="timedOut">Indicates whether the process timed out.</param>
			/// <param name="duration">The amount of time the job app was running.</param>
			public Task ExitApp(int exitCode, bool timedOut, long duration) =>
				SendPriorityAsync(nameof(ExitApp), exitCode, timedOut, duration);

			/// <summary>
			/// Call made when an unexpected error happens on the node while running a job.
			/// </summary>
			/// <param name="details">The details of the error.</param>
			public Task FailApp(object details) => SendPriorityAsync(nameof(FailApp), details);

			/// <summary>
			/// Call made by the job execution to set the expected total number of items.
			/// </summary>
			/// <param name="total">The expected total number of items.</param>
			public Task SetTotal(int total) => SendPriorityAsync(nameof(SetTotal), total);

			/// <summary>
			/// Call made by the job execution to broadcast an update of state and item progress.
			/// </summary>
			/// <param name="state">The state of the job.</param>
			/// <param name="categories">The progress of the job.</param>
			public Task UpdateState(InstanceState state, params ItemProgress[] categories) =>
				SendPriorityAsync(nameof(UpdateState), state, (IEnumerable<ItemProgress>)categories);

			/// <summary>
			/// Call made by the job execution to broadcast an update of state and item progress.
			/// </summary>
			/// <param name="state">The state of the job.</param>
			/// <param name="categories">The progress of the job.</param>
			public Task UpdateState(InstanceState state, IEnumerable<ItemProgress> categories) =>
				SendPriorityAsync(nameof(UpdateState), state, categories.ToArray());

			/// <summary>
			/// Call made by the job execution when a job method has been invoked.
			/// </summary>
			/// <param name="outcome">The outcome of invoking the job method.</param>
			public Task MethodResult(MethodOutcome outcome) => SendPriorityAsync(nameof(MethodResult), outcome);

			/// <summary>
			/// Call made by the job execution when an item has been processed.
			/// </summary>
			/// <param name="result">The result of processing the item.</param>
			public Task ItemResult(ItemResult result) => SendAsync(nameof(ItemResult), result);

			/// <summary>
			/// Call made by the node when data is read from the standard output or error of the job app.
			/// </summary>
			/// <param name="type">Indicates whether the data is from the standard output or error.</param>
			/// <param name="index">Sequencing index for this message.</param>
			/// <param name="log">The data to log.</param>
			public Task Log(RunLogType type, int index, string log) => SendAsync(nameof(Log), type, index, log);

			/// <summary>
			/// Call made by the job execution when the job is complete.
			/// </summary>
			/// <param name="disposition">The <see cref="Disposition"/> of the job execution.</param>
			public Task MarkComplete(Disposition disposition)
				=> MarkComplete(disposition, null);

			/// <summary>
			/// Call made by the job execution when the job is complete.
			/// </summary>
			/// <param name="disposition">The <see cref="Disposition"/> of the job execution.</param>
			/// <param name="output">The output from <see cref="IJob.FinalizeAsync(Disposition)"/>.</param>
			/// <param name="categories">The final item progress.</param>
			public Task MarkComplete(Disposition disposition, object output, params ItemProgress[] categories)
				=> MarkComplete(disposition, output, (IEnumerable<ItemProgress>)categories);

			/// <summary>
			/// Call made by the job execution when the job is complete.
			/// </summary>
			/// <param name="disposition">The <see cref="Disposition"/> of the job execution.</param>
			/// <param name="output">The output from <see cref="IJob.FinalizeAsync(Disposition)"/>.</param>
			/// <param name="categories">The final item progress.</param>
			public Task MarkComplete(Disposition disposition, object output, IEnumerable<ItemProgress> categories) =>
				SendPriorityAsync(nameof(MarkComplete), disposition, output, categories.ToArray());


			/// <summary>
			/// Disposes the <see cref="Connection"/>.
			/// </summary>
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

	/// <summary>
	/// Represents a SignalR hub method call.
	/// </summary>
	public class HubMethodCall
	{
		/// <summary>
		/// The number of attempts made to call this method.
		/// </summary>
		public int Attempts { get; set; }

		/// <summary>
		/// The server method to be invoked.
		/// </summary>
		public string MethodName { get; }

		/// <summary>
		/// The arguments to be supplied to the server method.
		/// </summary>
		public object[] Args { get; }

		/// <summary>
		/// Initializes a new <see cref="HubMethodCall"/>.
		/// </summary>
		/// <param name="methodName">The server method to be invoked.</param>
		/// <param name="args">The arguments to be supplied to the server method.</param>
		public HubMethodCall(string methodName, params object[] args)
		{
			this.MethodName = methodName;
			this.Args = args;
		}

		/// <summary>
		/// Returns the <see cref="MethodName"/>.
		/// </summary>
		public override string ToString() => MethodName;
	}

	/// <summary>
	/// An interface to make the SignalR <see cref="HubConnection"/> testable.
	/// </summary>
	public interface IResultsConnection : IAsyncDisposable
	{
		/// <summary>
		/// Occurs when the connection is closed. The connection could be closed due to an
		/// error or due to either the server or client intentionally closing the connection
		/// without error.
		/// </summary>
		/// <remarks>
		/// If this event was triggered from a connection error, the System.Exception that
		/// occurred will be passed in as the sole argument to this handler.If this event
		/// was triggered intentionally by either the client or server, then the argument
		/// will be null.
		/// </remarks>
		event Func<Exception, Task> Closed;

		/// <summary>
		/// Occurs when the Microsoft.AspNetCore.SignalR.Client.HubConnection starts reconnecting
		/// after losing its underlying connection.
		/// </summary>
		/// <remarks>
		/// The System.Exception that occurred will be passed in as the sole argument to
		/// this handler.
		/// </remarks>
		event Func<string, Task> Reconnected;

		/// <summary>
		/// Occurs when the Microsoft.AspNetCore.SignalR.Client.HubConnection starts reconnecting
		/// after losing its underlying connection.
		/// </summary>
		/// <remarks>
		/// The System.Exception that occurred will be passed in as the sole argument to
		/// this handler.
		/// </remarks>
		event Func<Exception, Task> Reconnecting;

		/// <summary>
		/// Indicates the state of the <see cref="HubConnection"/> to the server.
		/// </summary>
		HubConnectionState State { get; }

		/// <summary>
		/// Starts a connetion to the server.
		/// </summary>
		Task StartAsync();

		/// <summary>
		/// Invokes a hub method on the server.
		/// </summary>
		/// <param name="method">The name of the server method to invoke.</param>
		/// <param name="args">The arguments used to invoke the server method.</param>
		Task InvokeCoreAsync(string method, object[] args);

		/// <summary>
		/// Invokes a hub method on the server. Does not wait to a response from the receiver.
		/// </summary>
		/// <param name="method">The name of the server method to invoke.</param>
		/// <param name="args">The arguments used to invoke the server method.</param>
		Task SendCoreAsync(string method, object[] args);

		/// <summary>
		/// Registers a handler that will be invoked when the hub method with the specified name is invoked.
		/// </summary>
		/// <param name="methodName">The name of the hub method to define.</param>
		/// <param name="handler">The handler that will be raised when the hub method is invoked.</param>
		IDisposable On(string methodName, Action handler);
	}

	/// <summary>
	/// Wraps a SignalR <see cref="HubConnection"/> in the testable interface <see cref="IResultsConnection"/>.
	/// </summary>
	public class HubConnectionWrapper : IResultsConnection
	{
		readonly HubConnection connection;

		/// <summary>
		/// Initializes a new <see cref="HubConnectionWrapper"/>.
		/// </summary>
		/// <param name="connection"></param>
		public HubConnectionWrapper(HubConnection connection)
		{
			this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		/// <summary>
		/// Occurs when the connection is closed. The connection could be closed due to an
		/// error or due to either the server or client intentionally closing the connection
		/// without error.
		/// </summary>
		/// <remarks>
		/// If this event was triggered from a connection error, the System.Exception that
		/// occurred will be passed in as the sole argument to this handler.If this event
		/// was triggered intentionally by either the client or server, then the argument
		/// will be null.
		/// </remarks>
		public event Func<Exception, Task> Closed
		{
			add { connection.Closed += value; }
			remove { connection.Closed -= value; }
		}

		/// <summary>
		/// Occurs when the Microsoft.AspNetCore.SignalR.Client.HubConnection successfully
		/// reconnects after losing its underlying connection.
		/// </summary>
		/// <remarks>
		/// The System.String parameter will be the Microsoft.AspNetCore.SignalR.Client.HubConnection's
		/// new ConnectionId or null if negotiation was skipped.
		/// </remarks>
		public event Func<string, Task> Reconnected
		{
			add { connection.Reconnected += value; }
			remove { connection.Reconnected -= value; }
		}

		/// <summary>
		/// Occurs when the Microsoft.AspNetCore.SignalR.Client.HubConnection starts reconnecting
		/// after losing its underlying connection.
		/// </summary>
		/// <remarks>
		/// The System.Exception that occurred will be passed in as the sole argument to
		/// this handler.
		/// </remarks>
		public event Func<Exception, Task> Reconnecting
		{
			add { connection.Reconnecting += value; }
			remove { connection.Reconnecting -= value; }
		}

		/// <summary>
		/// Indicates the state of the <see cref="HubConnection"/> to the server.
		/// </summary>
		public HubConnectionState State => connection.State;

		/// <summary>
		/// Disposes the <see cref="HubConnectionWrapper"/>.
		/// </summary>
		public async ValueTask DisposeAsync()
		{
			if (connection != null)
				await connection.DisposeAsync();
		}

		/// <summary>
		/// Starts a connetion to the server.
		/// </summary>
		public Task StartAsync() => connection.StartAsync();

		/// <summary>
		/// Invokes a hub method on the server.
		/// </summary>
		/// <param name="methodName">The name of the server method to invoke.</param>
		/// <param name="args">The arguments used to invoke the server method.</param>
		public Task InvokeCoreAsync(string methodName, object[] args) => connection.InvokeCoreAsync(methodName, args);

		/// <summary>
		/// Invokes a hub method on the server. Does not wait to a response from the receiver.
		/// </summary>
		/// <param name="methodName">The name of the server method to invoke.</param>
		/// <param name="args">The arguments used to invoke the server method.</param>
		public Task SendCoreAsync(string methodName, object[] args) => connection.SendCoreAsync(methodName, args);

		/// <summary>
		/// Registers a handler that will be invoked when the hub method with the specified name is invoked.
		/// </summary>
		/// <param name="methodName">The name of the hub method to define.</param>
		/// <param name="handler">The handler that will be raised when the hub method is invoked.</param>
		public IDisposable On(string methodName, Action handler) => connection.On(methodName, handler);
	}
}
