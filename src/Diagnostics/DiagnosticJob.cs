using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Diagnostics
{
	public class DiagnosticJob : Job<DiagnosticConfig, DiagnosticItem>
	{
		readonly AutoResetEvent signal = new AutoResetEvent(false);
		public DateTime? InitializedAt { get; private set; }
		public DateTime? ItemsRetrievedAt { get; private set; }
		public DateTime? ProcessedAt { get; private set; }
		public DateTime? FinalizedAt { get; private set; }

		public DiagnosticJob(DiagnosticConfig config)
			: base(config) { }

		public void Signal()
		{
			signal.Set();
		}

		public override Task InitializeAsync()
		{
			if (Config.WaitForSignalInInitializeAsync)
				signal.WaitOne();

			this.InitializedAt = DateTime.Now;

			if (Config.ThrowExceptionInInitializeAsync)
				throw new DiagnosticJobException(JobMethod.InitializeAsync, "Exception thrown because Config.ThrowExceptionInInitialize is true.");

			return Task.CompletedTask;
		}

		public override IAsyncEnumerable<DiagnosticItem> GetItemsAsync()
		{
			this.ItemsRetrievedAt = DateTime.Now;

			if (Config.ThrowExceptionInGetItemsAsync)
				throw new DiagnosticJobException(JobMethod.GetItemsAsync, "Exception thrown because Config.ThrowExceptionInGetItemsToProcess is true.");

			if (Config.Categories == null || Config.Categories.Length == 0)
				Config.Categories = new DiagnosticConfig.Category[]
				{	
					new DiagnosticConfig.Category
					{
						IsSuccessful = true,
						Name = Result.Successful,
						Count = Config.NumberOfItems
					}
				};

			var items = new DiagnosticItem[Config.Categories.Sum(c => c.Count)];
			int i = 0;

			foreach (var cat in Config.Categories)
			{
				for (int j = 0; j < cat.Count; j++)
				{
					items[i] = new DiagnosticItem
					{
						Id = (i++).ToString(),
						Category = cat.Name ?? (cat.IsSuccessful ? Result.Successful : Result.Failed),
						IsSuccessful = cat.IsSuccessful
					};
				}
			}

			if (Config.ThrowExceptionInGetEnumerator)
				return new DiagnosticEnumerable<DiagnosticItem>(MethodResponse.ThrowException, MethodResponse.ValidValue, MethodResponse.ValidValue).ToAsyncEnumerable(Config.CanCountItems);
			else if (Config.ThrowExceptionInEnumeratorMoveNext)
				return new DiagnosticEnumerable<DiagnosticItem>(MethodResponse.ValidValue, MethodResponse.ThrowException, MethodResponse.ValidValue).ToAsyncEnumerable(Config.CanCountItems);
			else if (Config.ThrowExceptionInEnumeratorCurrent)
				return new DiagnosticEnumerable<DiagnosticItem>(MethodResponse.ValidValue, MethodResponse.ValidValue, MethodResponse.ThrowException).ToAsyncEnumerable(Config.CanCountItems);
			else
				return items.ToAsyncEnumerable(Config.CanCountItems);
		}

		public override Task<string> GetItemIdAsync(DiagnosticItem item)
		{
			if (Config.ThrowExceptionInGetItemIdAsync)
				throw new DiagnosticJobException(JobMethod.GetItemIdAsync, "Exception thrown because Config.ThrowExceptionInGetItemsAsync is true.");

			return Task.FromResult(item.Id);
		}

		public override Task<Result> ProcessAsync(DiagnosticItem item)
		{
			if (Config.MillisecondDelayPerItem > 0)
				Thread.Sleep(Config.MillisecondDelayPerItem);

			if (Config.WaitForSignalInProcessAsync)
				signal.WaitOne();

			if (ProcessedAt is null)
				ProcessedAt = DateTime.Now;

			if (!string.IsNullOrWhiteSpace(Config.MessageToLogInProcessAsync))
				Console.WriteLine($"{item.Id}: {Config.MessageToLogInProcessAsync}");

			if (Config.ThrowExceptionInProcessAsync)
				throw new DiagnosticJobException(JobMethod.ProcessAsync, "Exception thrown because Config.ThrowExceptionInProcess is true.");

			return Task.FromResult(item.IsSuccessful ? Result.Success(item.Category) : Result.Failure(item.Category));
		}

		public override Task<object> FinalizeAsync(Disposition disposition)
		{
			if (Config.WaitForSignalInFinalizeAsync)
				signal.WaitOne();

			this.FinalizedAt = DateTime.Now;

			if (Config.ThrowExceptionInFinalizeAsync)
				throw new DiagnosticJobException(JobMethod.FinalizeAsync, "Exception thrown because Config.ThrowExceptionInFinalize is true.");

			return base.FinalizeAsync(disposition);
		}
	}
}
