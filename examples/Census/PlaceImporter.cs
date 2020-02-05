using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace Runly.Examples.Census
{
	public class PlaceImporter : Process<CensusConfig, Place>, IDisposable
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

			var reader = new StreamReader(data);
			csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			csv.Configuration.Delimiter = "|";
		}

		public override Task<IEnumerable<Place>> GetItemsAsync()
		{
			logger.LogInformation("Parsing CSV file...");

			// use ToList to parse the entire file right here so we can process the items on multiple threads
			IEnumerable<Place> items = csv.GetRecords<Place>().ToList();

			return Task.FromResult(items);
		}

		public override Task<Result> ProcessAsync(Place place)
		{
			return Task.FromResult(Result.Success(place.State));
		}

		public void Dispose()
		{
			csv?.Dispose();
		}
	}
}
