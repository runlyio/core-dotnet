using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runly
{
	/// <summary>
	/// Logs results from a <see cref="IJob"/>.
	/// </summary>
	public class ResultLog
	{
		readonly Execution execution;
		readonly ConcurrentBag<ItemResult> items = new ConcurrentBag<ItemResult>();
		
		/// <summary>
		/// The file path of the config used by the run.
		/// </summary>
		public string ConfigPath { get; internal set; }

		/// <summary>
		/// The UTC time at which the run started.
		/// </summary>
		public DateTime? StartedAt { get; internal set; }

		/// <summary>
		/// The UTC time at which the run ended.
		/// </summary>
		public DateTime? CompletedAt { get; internal set; }

		/// <summary>
		/// The final state of the run.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public Disposition Disposition { get; internal set; }

		/// <summary>
		/// Gets a list of <see cref="MethodOutcome">MethodOutcomes</see> for each method of a job, excluding methods executed per item.
		/// </summary>
		public Dictionary<JobMethod, MethodOutcome> Methods { get; } = new Dictionary<JobMethod, MethodOutcome>();

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for InitializeAsync.
		/// </summary>
		[JsonIgnore]
		public MethodOutcome InitializeAsync => Methods.ValueOrDefault(JobMethod.InitializeAsync);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for GetItemsAsync.
		/// </summary>
		[JsonIgnore]
		public MethodOutcome GetItemsAsync => Methods.ValueOrDefault(JobMethod.GetItemsAsync);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for Count.
		/// </summary>
		[JsonIgnore]
		public MethodOutcome Count => Methods.ValueOrDefault(JobMethod.Count);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for GetEnumerator.
		/// </summary>
		[JsonIgnore]
		public MethodOutcome GetEnumerator => Methods.ValueOrDefault(JobMethod.GetEnumerator);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for FinalizeAsync.
		/// </summary>
		[JsonIgnore]
		public MethodOutcome FinalizeAsync => Methods.ValueOrDefault(JobMethod.FinalizeAsync);

		/// <summary>
		/// Individual items results
		/// </summary>
		public IEnumerable<ItemResult> Items => items;

		/// <summary>
		/// Gets a list of item categories and counts.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<ItemCategory> Categories => Items.GroupBy(r => new { r.IsSuccessful, r.Category }).Select(g => new ItemCategory(g.Count(), g.Key.IsSuccessful, g.Key.Category)).OrderBy(s => s.IsSuccessful).ThenBy(s => s.Category);


		/// <summary>
		/// Gets a list of items that have completed successfully.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<ItemResult> SuccessfulItems => Items.Where(r => r.IsSuccessful);

		/// <summary>
		/// Gets a list of items that completed unsuccessfully.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<ItemResult> FailedItems => Items.Where(r => !r.IsSuccessful);

		/// <summary>
		/// Gets a list of <see cref="FailedItems"/> that threw an exception instead of returning a <see cref="Result"/>.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<ItemResult> FailedItemsThatThrewExceptions => FailedItems.Where(r => r.FailedDueToException);

		/// <summary>
		/// The total number of items to be processed.
		/// </summary>
		/// <remarks>
		/// <see cref="TotalItemCount"/> will be null when a <see cref="IJob"/> sets canBeCounted to false when using ToAsyncEnumerable. The difference between
		/// this total and the sum of <see cref="SuccessfulItemCount"/> and <see cref="FailedItemCount"/> is the number of items
		/// yet to be processed or not processed in the case of a job that ended in the <see cref="Disposition.Failed"/> state.
		/// </remarks>
		public int? TotalItemCount { get; internal set; }

		/// <summary>
		/// The number of items processed with a successful (<see cref="Result.IsSuccessful"/>) <see cref="Result"/>.
		/// </summary>
		public int SuccessfulItemCount => SuccessfulItems.Count();

		/// <summary>
		/// The number of items processed with an unsuccessful (not <see cref="Result.IsSuccessful"/>) <see cref="Result"/>.
		/// </summary>
		public int FailedItemCount => FailedItems.Count();      
		
		/// <summary>
		/// The output from FinalizeAsync
		/// </summary>
		public object Output { get; internal set; }

		/// <summary>
		/// Initializes a new <see cref="ResultLog"/>.
		/// </summary>
		public ResultLog(Execution execution)
		{
			this.execution = execution ?? throw new ArgumentNullException(nameof(execution));
			this.execution.Started += Started;
			this.execution.MethodCompleted += MethodCompleted;
			this.execution.ItemCompleted += ItemCompleted;
			this.execution.Completed += Completed;
		}

		/// <summary>
		/// Initializes a new <see cref="ResultLog"/>.
		/// </summary>
		public ResultLog(IEnumerable<ItemResult> items, Disposition disposition, int? totalItemCount, Dictionary<JobMethod, MethodOutcome> methods, object output)
		{
			this.items = new ConcurrentBag<ItemResult>(items ?? new ItemResult[0]);
			Disposition = disposition;
			TotalItemCount = totalItemCount;
			Methods = methods;
			Output = output;
		}

		private Task Started(string configPath, DateTime startedAt)
		{
			StartedAt = startedAt;
			ConfigPath = configPath;

			return Task.CompletedTask;
		}

		private Task MethodCompleted(MethodOutcome method)
		{
			Methods.Add(method.Method, method);

			return Task.CompletedTask;
		}

		private Task ItemCompleted(ItemResult result)
		{
			items.Add(result);

			return Task.CompletedTask;
		}

		private Task Completed(object output, Disposition disposition, DateTime completedAt)
		{
			Output = output;
			Disposition = disposition;
			CompletedAt = completedAt;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Produces a report of this job for the console.
		/// </summary>
		public override string ToString()
		{
			var report = new StringBuilder();

			report.AppendLine(ConsoleFormat.DoubleLine);
			report.AppendLine($"JOB {Disposition.ToString().ToUpper()}");
			report.AppendLine(ConsoleFormat.DoubleLine);
			report.AppendLine();
			report.AppendLine($"{Items.Count()} item{ConsoleFormat.AsPlural(Items.Count())} {(TotalItemCount.HasValue ? "of " + TotalItemCount + " processed" : "processed")}");
			report.AppendLine();
			report.AppendLine(ConsoleFormat.AsColumns(20, nameof(InitializeAsync), InitializeAsync.IsSuccessful ? "Successful" : "Unsuccessful"));
			report.AppendLine(ConsoleFormat.AsColumns(20, nameof(GetItemsAsync), GetItemsAsync.IsSuccessful ? "Successful" : "Unsuccessful"));
			if (!Count?.IsSuccessful ?? false) report.AppendLine(ConsoleFormat.AsColumns(20, nameof(Count), Count.IsSuccessful ? "Successful" : "Unsuccessful"));
			if (!GetEnumerator?.IsSuccessful ?? false) report.AppendLine(ConsoleFormat.AsColumns(20, nameof(GetEnumerator), GetEnumerator.IsSuccessful ? "Successful" : "Unsuccessful"));
			report.AppendLine(ConsoleFormat.AsColumns(20, "ProcessAsync", FailedItemCount == 0 ? "Successful" : "Unsuccessful"));
			report.AppendLine(ConsoleFormat.AsColumns(20, nameof(FinalizeAsync), FinalizeAsync.IsSuccessful ? "Successful" : "Unsuccessful"));
			report.AppendLine();

			foreach (var summary in Categories)
				report.AppendLine(ConsoleFormat.AsColumns(20, $"{summary.Count} item{ConsoleFormat.AsPlural(Items.Count())}", summary.IsSuccessful ? "Successful" : "Unsuccessful", summary.Category));

			report.AppendLine();

			if (FailedItemsThatThrewExceptions.Count() > 0 || Methods.Values.Any(m => !m.IsSuccessful))
			{
				report.AppendLine(ConsoleFormat.DoubleLine);
				report.AppendLine("UNHANDLED EXCEPTIONS");
				report.AppendLine(ConsoleFormat.DoubleLine);
				report.AppendLine();

				foreach (var method in Methods.Values.Where(m => !m.IsSuccessful))
				{
					report.AppendLine(ConsoleFormat.SingleLine);
					report.AppendLine(method.Method.ToString());
					report.AppendLine(ConsoleFormat.SingleLine);
					report.AppendLine(method.Exception.ToString());
					report.AppendLine();
				}

				foreach (var failure in FailedItemsThatThrewExceptions.Take(10))
				{
					report.AppendLine(ConsoleFormat.SingleLine);
					report.AppendLine(failure.Id ?? "Unknown");
					report.AppendLine(ConsoleFormat.SingleLine);
					report.AppendLine((failure.ProcessAsync.Exception ?? failure.EnumeratorCurrent.Exception ?? failure.EnumeratorMoveNext.Exception).ToString());
					report.AppendLine();
				}
			}

			return report.ToString();
		}
	}
}
