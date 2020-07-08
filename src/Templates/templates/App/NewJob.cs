using Runly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewRunlyApp
{
	// Quickstart guide to building a job: https://www.runly.io/docs/building/
	// Dive deep into the anatomy of a Job: https://www.runly.io/docs/jobs/
	public class NewJob : Job<NewJobConfig, string>
	{
		public NewJob(NewJobConfig config)
			: base(config) { }

		public override IAsyncEnumerable<string> GetItemsAsync()
		{
			// Return a collection of items for your job to process.

			// To change the item type returned from this method, change the second generic
			// type parameter of the extended Job class, which is 'string' in this template.

			// To return a collection that is an IEnumerable<T> or Task<IEnumerable<T>>,
			// use the extension method ToAsyncEnumerable() found in the Runly namespace:
			// return new string[] { "thing1", "thing2", "thing3" }.ToAsyncEnumerable();

			// If your data source requires awaiting each item, which may be the case with
			// an API or file, take advantage of 'yield return await' in a foreach loop:
			// foreach (var id in await api.GetUnfulfilledOrderIds())
			//    yield return await api.GetOrder(id);

			throw new NotImplementedException();
		}

		public override async Task<Result> ProcessAsync(string item)
		{
			// Do the work to process each item.

			// To add additional parameters to this method, to take an database connection 
			// with the interface 'IDatabase' for example, do the following:
			// 1. Add the parameter to this method:
			// public override async Task<Result> ProcessAsync(string item, IDatabase db)
			// 2. Add the parameter type to the generic type parameters of the Job class
			// being extended above:
			// public class NewJob : Job<NewJobConfig, string, IDatabase>
			// 3. Configure an implementation of IDatabase in the Main method in Program.cs:
			// await JobHost.CreateDefaultBuilder(args)
			//    .ConfigureServices((c, s) => s.AddScoped<Database>())
			//    .Build()
			//    .RunJobAsync();

			// Processing an item requires a Result indicating success or failure. 
			// - If all is well and there's nothing interesting to report: return Result.Success();
			// - If there are different categories of success, report that by setting the category:
			// return Result.Success("No changes");
			// - If there is data to capture for users or other apps to consume, add output:
			// Result.Success("No changes", new { Before = oldval, After = newval });
			// - If an item cannot be processed and the the Job should stop (depending on the Job's 
			// Config.Execution.ItemFailureCountToStopJob), indicate failure:
			// Result.Failure("Could not connect to API");

			return Result.Success();
		}
	}
}
