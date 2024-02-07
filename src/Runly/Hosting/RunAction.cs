using Microsoft.Extensions.Logging;
using Runly.Client;
using Runly.Client.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Runly.Hosting
{
	class RunAction : IHostAction
	{
		readonly Execution execution;
		readonly Config config;
		readonly ILogger<RunAction> logger;
		readonly ResultsChannel api;
		readonly bool debug;
		TimeSpan processDuration = new TimeSpan();

		public RunAction(Execution execution, Config config, ILogger<RunAction> logger, ResultsChannel api, Debug debug)
		{
			this.execution = execution;
			this.config = config;
			this.logger = logger;
			this.api = api;
			this.debug = debug?.AttachDebugger ?? config?.Execution?.LaunchDebugger ?? false;
		}

		public async Task RunAsync(CancellationToken token)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            AnsiConsole.MarkupLine($"Running [blue]{config.Job.Type}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]Press 'c' to cancel[/]");
            AnsiConsole.WriteLine();

			if (debug)
				AttachDebugger();

			if (api != null && string.IsNullOrWhiteSpace(config.RunlyApi.OrganizationId))
			{
				var message = "OrganizationId is required when an API token is provided.";
				logger.LogError(message);
				throw new ConfigException(message, nameof(Config.RunlyApi));
			}

			if (config.RunlyApi.InstanceId != Guid.Empty && (api == null || string.IsNullOrWhiteSpace(config.RunlyApi.OrganizationId)))
			{
				var message = "An API token and OrganizationId are required when InstanceId is specified.";
				logger.LogError(message);
				throw new ConfigException(message, nameof(Config.RunlyApi));
			}

			var jobCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
			var pingCancellation = new CancellationTokenSource();
			var useApi = config.RunlyApi.InstanceId != Guid.Empty;
			ResultsChannel.Connection channel = null;
			ResultLog log = null;

			try
			{
				if (useApi)
				{
					channel = await api.ConnectAsync(config.RunlyApi.InstanceId);

					channel.CancellationRequested += () =>
					{
						jobCancellation.Cancel();
						return Task.CompletedTask;
					};
					execution.StateChanged += s => UpdateInstanceState(channel);
					execution.StateChanged += s =>
					{
						if (s == ExecutionState.Processing && execution.TotalItemCount.HasValue)
							return channel.SetTotal(execution.TotalItemCount.Value);
						else if (s == ExecutionState.Finalizing)
							return channel.MethodResult(new MethodOutcome(JobMethod.ProcessAsync, processDuration, (Error)null));
						else
							return Task.CompletedTask;
					};
					execution.MethodCompleted += m => channel.MethodResult(m);
					execution.ItemCompleted += i =>
					{
						processDuration += TimeSpan.FromMilliseconds(i.Methods.Sum(m => m.Value.Duration.TotalMilliseconds));

						if (config.RunlyApi.LogSuccessfulItemResults || !i.IsSuccessful)
							return channel.ItemResult(i);
						else
							return Task.CompletedTask;
					};
					execution.Completed += (output, disposition, completedAt) => channel.MarkComplete(disposition, output, execution.ItemCategories.Select(c => new ItemProgress
					{
						Category = c.Category,
						IsSuccessful = c.IsSuccessful,
						Count = c.Count
					}));
				}

				if (config.Execution.ResultsToConsole || config.Execution.ResultsToFile)
					log = new ResultLog(execution);

				var executing = execution.ExecuteAsync(jobCancellation.Token);
				var pinging = Task.CompletedTask;

				if (useApi)
					pinging = PingApi(channel, pingCancellation.Token);

				if (config.Execution.ResultsToConsole)
				{
                    await AnsiConsole.Status()
						.Spinner(Spinner.Known.Dots)
						.SpinnerStyle(Style.Parse("yellow bold"))
						.StartAsync("Running", async ctx =>
						{
							AnsiConsole.Markup("Initializing...");

							while (execution.State == ExecutionState.NotStarted ||
								execution.State == ExecutionState.Initializing)
								await Task.Delay(10);

							if (execution.InitializeAsync.IsSuccessful)
							{
								AnsiConsole.MarkupLine("[green]Success[/]");

                                AnsiConsole.Markup("Getting items...");

                                while (execution.State == ExecutionState.GettingItemsToProcess)
									await Task.Delay(10);

								if (execution.GetItemsAsync.IsSuccessful && execution.GetEnumerator.IsSuccessful && execution.Count.IsSuccessful)
								{
									AnsiConsole.MarkupLine("[green]Success[/]");
								}
								else
								{
									AnsiConsole.MarkupLine("[red]Failed[/]");
									AnsiConsole.WriteLine();
									AnsiConsole.WriteException(execution.GetItemsAsync.Exception ?? execution.GetEnumerator.Exception ?? execution.Count.Exception);
									AnsiConsole.WriteLine();
								}
                            }
							else
							{
								AnsiConsole.MarkupLine("[red]Failed[/]");
                                AnsiConsole.WriteLine();
                                AnsiConsole.WriteException(execution.InitializeAsync.Exception);
								AnsiConsole.WriteLine();
							}

                        });

					if (execution.Disposition != Disposition.Failed)
					{
						var isIndeterminate = !execution.TotalItemCount.HasValue;

						var cols = isIndeterminate ?
						new ProgressColumn[]
						{
							new TaskDescriptionColumn(),
							new AverageTimeColumn(execution),
							new AverageTimeLastMinColumn(execution),
							new ElapsedTimeColumn() { Style = Style.Parse("dim") },
							new SpinnerColumn(Spinner.Known.Dots),
						} :
						new ProgressColumn[]
						{
							new TaskDescriptionColumn(),
                            new AverageTimeColumn(execution),
                            new AverageTimeLastMinColumn(execution),
                            new PercentageColumn(),
                            new ElapsedTimeColumn() { Style = Style.Parse("dim") },
                            new SpinnerColumn(Spinner.Known.Dots),
                        };

                        await AnsiConsole.Progress()
							.Columns(cols)
							.StartAsync(async ctx =>
							{
								var process = ctx.AddTask(isIndeterminate ? "Processing items:" : $"Processing {execution.TotalItemCount:N0} items:", maxValue: execution.TotalItemCount ?? double.MaxValue);

								process.IsIndeterminate = isIndeterminate;

                                while (execution.State == ExecutionState.Processing)
								{
									process.Value = execution.CompletedItemCount;

                                    await Task.Delay(100);

                                    if (!Console.IsInputRedirected)
									{
										while (Console.KeyAvailable)
										{
											var key = Console.ReadKey();

											if (key.KeyChar == 'c' || key.KeyChar == 'C')
												jobCancellation.Cancel();
                                        }
									}										
								}

                                await Task.Delay(100);
                                ctx.Refresh();

                                process.Value = execution.CompletedItemCount;
								process.StopTask();
							});
					}

                    await AnsiConsole.Status()
                        .Spinner(Spinner.Known.Dots)
                        .SpinnerStyle(Style.Parse("yellow bold"))
                        .StartAsync("Running", async ctx =>
                        {
                            if (log.Categories.Any())
                            {
								AnsiConsole.MarkupLine("[dim]Item results:[/]");

                                var grid = new Grid();
                                grid.AddColumn();
                                grid.AddColumn();

                                foreach (var c in log.Categories)
                                {
                                    grid.AddRow(new Markup[]
                                    {
										new Markup($"{c.Count}"),
										new Markup($"{(c.IsSuccessful ? "" : "[red]")}{c.Category}{(c.IsSuccessful ? "" : "[/]")}")
									});
                                }

                                AnsiConsole.Write(grid);
                                AnsiConsole.WriteLine();
                            }

							if (log.FailedItemCount > 0)
							{
								foreach (var failure in log.FailedItemsThatThrewExceptions.Take(10))
								{
									AnsiConsole.MarkupLine($"Item '{failure.Id ?? "Unknown"}' threw an exception:");
									AnsiConsole.WriteException(failure.ProcessAsync?.Exception ?? failure.GetItemIdAsync?.Exception ?? failure.EnumeratorCurrent?.Exception ?? failure.EnumeratorMoveNext?.Exception);
								}

								AnsiConsole.WriteLine();
							}

                            AnsiConsole.Markup("Finalizing...");

                            while (execution.State == ExecutionState.Finalizing)
                                await Task.Delay(10);

							if (execution.FinalizeAsync.IsSuccessful)
							{
								AnsiConsole.MarkupLine("[green]Success[/]");
							}
							else
							{
								AnsiConsole.MarkupLine("[red]Failed[/]");
								AnsiConsole.WriteLine();
								AnsiConsole.WriteException(execution.FinalizeAsync.Exception);
								AnsiConsole.WriteLine();
							}
                        });
                }



                // Execution.RunAsync will ensure all event handlers have completed before exiting
                await executing;

				pingCancellation.Cancel();

				if (config.Execution.ResultsToConsole)
                {
                    AnsiConsole.WriteLine();

					if (execution.Disposition == Disposition.Successful)
						AnsiConsole.MarkupLine($"[bold green]JOB SUCCESSFUL[/]");
					else if (execution.Disposition == Disposition.Cancelled)
						AnsiConsole.MarkupLine($"[bold darkorange]JOB CANCELLED[/]");
					else
                        AnsiConsole.MarkupLine($"[bold red invert]JOB FAILED[/]");

                }

                if (config.Execution.ResultsToFile)
				{
					using var writer = new StreamWriter(File.Open(config.Execution.ResultsFilePath, FileMode.Create));

					writer.WriteJson(log);
					await writer.FlushAsync();
				}

				try
				{
					await pinging;
				}
				catch (TaskCanceledException) { }

				if (channel != null)
					await channel.FlushAsync();
			}
			finally
			{
				if (channel != null)
					await channel.DisposeAsync();
			}

			// Ensure the entire output can be read by the node
			await Console.Out.FlushAsync();
		}

		/// <summary>
		/// Calls <see cref="ResultsChannel.Connection.UpdateState(InstanceState, IEnumerable{ItemProgress})"/>
		/// every 1 to 30 seconds, depending on whether new data is available.
		/// </summary>
		private async Task PingApi(ResultsChannel.Connection channel, CancellationToken cancellationToken)
		{
			var MinPingInterval = new TimeSpan(0, 0, 1);
			var MaxPingInterval = new TimeSpan(0, 0, 30);

			// Setting lastState to TimedOut because this will never come back from Execution.
			InstanceState lastState = InstanceState.TimedOut;
			int lastSuccessful = 0, lastFailed = 0;
			DateTime lastUpdate = DateTime.UtcNow.AddDays(-1);

			await Update();

			// Putting the delay first so that when cancellation 
			// is requested and update will occur before exiting
			while (!cancellationToken.IsCancellationRequested)
			{
				await Task.Delay(MinPingInterval, cancellationToken)
					.ContinueWith(tsk => {/* Don't throw exception on cancellation https://stackoverflow.com/a/32768637 */});

				await Update();
			}

			async Task Update()
			{
				try
				{
					await UpdateInstanceState(channel, (state, categories) =>
					{
						// Taking counts from the categories instead of SuccessfulItemCount and FailedItemCount
						// so that there is no different between these sums and the category sums due to changes
						// in the short duration between accessing different properties on execution.
						int successful = categories.Where(c => c.IsSuccessful).Sum(c => c.Count);
						int failed = categories.Where(c => !c.IsSuccessful).Sum(c => c.Count);

						bool shouldUpdate = (
							lastState != state ||
							lastSuccessful != successful ||
							lastFailed != failed ||
							DateTime.UtcNow.Subtract(lastUpdate) >= MaxPingInterval
						);

						if (shouldUpdate)
						{
							lastState = state;
							lastSuccessful = successful;
							lastFailed = failed;
							lastUpdate = DateTime.UtcNow;
						}

						return shouldUpdate;
					});
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error pinging API");
				}
			}
		}

		async Task UpdateInstanceState(ResultsChannel.Connection channel, Func<InstanceState, IEnumerable<ItemProgress>, bool> shouldUpdate = null)
		{
			var state = GetInstanceState(execution.State);
			var categories = execution.ItemCategories.Select(c => new ItemProgress
			{
				Category = c.Category,
				IsSuccessful = c.IsSuccessful,
				Count = c.Count
			});

			if (shouldUpdate == null || shouldUpdate(state, categories))
			{
				await channel.UpdateState(state, categories);
			}
		}

		private InstanceState GetInstanceState(ExecutionState state) => state switch
		{
			// Not casting ExecutionState to InstanceState since these could diverge in the future
			ExecutionState.NotStarted => InstanceState.NotStarted,
			ExecutionState.Initializing => InstanceState.Initializing,
			ExecutionState.GettingItemsToProcess => InstanceState.GettingItemsToProcess,
			ExecutionState.Processing => InstanceState.Processing,
			ExecutionState.Finalizing => InstanceState.Finalizing,
			ExecutionState.Complete =>
				execution.IsCanceled ? InstanceState.Cancelled :
				execution.IsFailed ? InstanceState.Failed :
				InstanceState.Successful,
			_ => throw new NotImplementedException("Unknown state")
		};

		private void AttachDebugger()
		{
			try
			{
				if (!Debugger.IsAttached)
				{
					Console.Write("Launching debugger...");

					if (Debugger.Launch() && Debugger.IsAttached)
						Console.WriteLine("attached.");
					else
						Console.WriteLine("failed to attach, continuing without debugger.");
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to launch debugger due to error:\n" + ex.ToString());
			}
		}
	}

    internal class ValueColumn : ProgressColumn
    {
		protected override bool NoWrap => true;

        /// <summary>
        /// Gets or sets the style of the remaining time text.
        /// </summary>
        public Style Style { get; set; } = Color.Blue;

        /// <inheritdoc/>
        public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
        {
            if (task.IsIndeterminate)
	            return new Text($"{task.Value:N0}", Style ?? Style.Plain);
			else
                return new Text($"{task.Value:N0}/{task.MaxValue:N0}", Style ?? Style.Plain);
        }
    }

    internal class AverageTimeColumn : ProgressColumn
    {
		private readonly Execution _execution;

		public AverageTimeColumn(Execution execution)
		{
			_execution = execution;
		}

        protected override bool NoWrap => true;

        /// <summary>
        /// Gets or sets the style of the remaining time text.
        /// </summary>
        public Style Style { get; set; } = Color.Blue;

        /// <inheritdoc/>
        public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
        {
            return new Markup($"[blue]{_execution.ItemProcessingTime.AllTime.Count:N0}[/] [dim]({_execution.ItemProcessingTime.AllTime.Average.TotalMilliseconds}ms avg)[/]");
        }
    }

    internal class AverageTimeLastMinColumn : ProgressColumn
    {
        private readonly Execution _execution;

        public AverageTimeLastMinColumn(Execution execution)
        {
            _execution = execution;
        }

        protected override bool NoWrap => true;

        /// <summary>
        /// Gets or sets the style of the remaining time text.
        /// </summary>
        public Style Style { get; set; } = Color.Blue;

        /// <inheritdoc/>
        public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
        {
			return new Markup($"Last min: [blue]{_execution.ItemProcessingTime.LastMinuteAverage.Count:N0}[/] [dim]({_execution.ItemProcessingTime.LastMinuteAverage.Average.TotalMilliseconds}ms avg)[/]");
        }
    }
}
