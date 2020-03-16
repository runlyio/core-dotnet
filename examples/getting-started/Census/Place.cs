using CsvHelper.Configuration.Attributes;

namespace Runly.GettingStarted.Census
{
	public class Place
	{

		[Name("STATE")]
		public string State { get; set; }

		[Name("COUNTY")]
		public string County { get; set; }

		[Name("PLACENAME")]
		public string Name { get; set; }

		[Name("TYPE")]
		public string Type { get; set; }

		public override string ToString() => Name;
	}
}
