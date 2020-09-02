using CsvHelper;
using Microsoft.Extensions.Logging;
using Runly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Examples.GettingStarted.Census
{
	/// <summary>
	/// "Imports" national places from a US Census publicly available CSV file. This job demonstrates
	/// processing a CSV file in parallel and doing something with the parsed data.
	/// </summary>
	/// <remarks>
	/// Makes use of OSS library CsvHelper.
	/// https://joshclose.github.io/CsvHelper/
	/// </remarks>
	public class PlaceImporter : Job<CensusConfig, Place, IDatabase>, IDisposable
	{
		readonly IDownloader downloader;
		readonly ILogger<PlaceImporter> logger;
		CsvReader csv;

		public PlaceImporter(CensusConfig config, IDownloader downloader, ILogger<PlaceImporter> logger)
			: base(config)
		{
			this.downloader = downloader;
			this.logger = logger;
		}

		public override async Task InitializeAsync()
		{
			var data = await downloader.Download("national_places.txt");

			csv = new CsvReader(new StreamReader(data), CultureInfo.InvariantCulture);
			csv.Configuration.Delimiter = "|";

			// We can use standard logging infrastrucure here if we so desire.
			logger.LogDebug("Finished downloading CSV file. Preparing to process...");
		}

		public override IAsyncEnumerable<Place> GetItemsAsync()
		{
			// Even though the CsvReader is not thread-safe (see https://github.com/JoshClose/CsvHelper/issues/908),
			// we can still stream the file using csv.GetRecordsAsync (without calling .ToList() first) since the job
			// will synchronize access to the enumerator so that only a single thread is reading/parsing a CSV record
			// at a time. This makes it easy to do multi-threaded processing even if not all of your dependencies
			// support it.
			return csv.GetRecordsAsync<Place>();
		}

		public override async Task<Result> ProcessAsync(Place place, IDatabase database)
		{
			// If a dependency is not thread-safe (in this case our fake IDatabase), we can take it as a
			// parameter in the ProcessAsync method instead of a constructor parameter. The job will resolve
			// a new instance of the dependency either per thread or per item (depending on if we registered
			// it as Transient or Scoped). Play around with the registration in ServiceExtensions to see how the
			// behavior changes (also change the log level of Runly.Examples to Debug).
			await database.SavePlace(place);

			return Result.Success(place.State);
		}

		public void Dispose()
		{
			csv?.Dispose();
		}
	}
}
