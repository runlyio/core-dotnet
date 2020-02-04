using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;

namespace Runly.Examples.Census
{
	public class PlaceImporter : Process<CensusConfig, Place>, IDisposable
	{
		readonly IDownloader downloader;
		CsvReader csv;

		public PlaceImporter(CensusConfig config, IDownloader downloader)
			: base(config)
		{
			this.downloader = downloader;

			// we want to stream the CSV file
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
