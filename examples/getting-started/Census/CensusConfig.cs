using Runly;

namespace Examples.GettingStarted.Census
{
	public class CensusConfig : Config
	{
		public string BaseUrl { get; set; } = "http://www2.census.gov/geo/docs/reference/codes/files/";
	}
}
