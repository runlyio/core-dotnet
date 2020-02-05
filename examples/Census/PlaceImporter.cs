using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace Runly.Examples.Census
{
	/// <summary>
	/// "Imports" national places from a US Census publicly available CSV file. This process demonstrates
	/// processing a CSV file in parallel and doing something with the parsed data.
	/// </summary>
	/// <remarks>
	/// Makes use of OSS library CsvHelper.
	/// https://joshclose.github.io/CsvHelper/
	/// </remarks>
	public class PlaceImporter : Process<CensusConfig, Place, IDatabase>, IDisposable
	{
		readonly IDownloader downloader;
		readonly ILogger<PlaceImporter> logger;
		CsvReader csv;

		public PlaceImporter(CensusConfig config, IDownloader downloader, ILogger<PlaceImporter> logger)
			: base(config)
		{
			this.downloader = downloader;
			this.logger = logger;

			// The CsvReader.GetRecords<T> method will return an IEnumerable<T> that will yield records.
			// What this means is that only a single record is returned at a time as the process iterates the records.
			// That also means that only a small portion of the file is read into memory. However, if we let the process
			// count the items (using .Count()), or do anything that executes a LINQ projection, such as calling .ToList(),
			// the entire file will be read into memory. We can disable the counting behavior so that the process will
			// stream the CSV file.
			Options.CanCountItems = false;
		}

		public override async Task InitializeAsync()
		{
			var data = await downloader.Download("national_places.txt");

			csv = new CsvReader(new StreamReader(data), CultureInfo.InvariantCulture);
			csv.Configuration.Delimiter = "|";

			// We can use standard logging infrastrucure here if we so desire.
			logger.LogDebug("Finished downloading CSV file. Preparing to process...");
		}

		public override Task<IEnumerable<Place>> GetItemsAsync()
		{
			// Even though the CsvReader is not thread-safe (see https://github.com/JoshClose/CsvHelper/issues/908),
			// we can still stream the file using csv.GetRecords (without calling .ToList() first) since the process
			// will synchronize access to the enumerator so that only a single thread is reading/parsing a CSV record
			// at a time. This makes it easy to do multi-threaded processing even if not all of your dependencies
			// support it.
			return Task.FromResult(csv.GetRecords<Place>());
		}

		public override async Task<Result> ProcessAsync(Place place, IDatabase database)
		{
			// If a dependency is not thread-safe (in this case our fake IDatabase), we can take it as a
			// parameter in the ProcessAsync method instead of a constructor parameter. The process will resolve
			// a new instance of the dependency either per thread or per item (depending on if we registered
			// it as Transient or Scoped). Play around with the registration in ServiceExtensions to see how the
			// behavior changes.
			await database.SavePlace(place);

			return Result.Success(place.State);
		}

		public void Dispose()
		{
			csv?.Dispose();
		}
	}
}
