using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace Runly.Examples.Census
{
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

			// we want to stream/read the CSV file as we are parsing it
			Options.CanCountItems = false;
		}

		public override async Task InitializeAsync()
		{
			var data = await downloader.Download("national_places.txt");

			var reader = new StreamReader(data);
			csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			csv.Configuration.Delimiter = "|";
		}

		public override Task<IEnumerable<Place>> GetItemsAsync() => Task.FromResult(csv.GetRecords<Place>());

		public override async Task<Result> ProcessAsync(Place place, IDatabase database)
		{
			// database is not a thread-safe dependency, resolve a new instance of it for each thread we process
			await database.SavePlace(place);

			return Result.Success(place.State);
		}

		public void Dispose()
		{
			csv?.Dispose();
		}
	}
}
