using Newtonsoft.Json;

namespace Runly.Models
{
	public class EnvironmentPackageUpdate
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> VersionRange { get; set; }
	}
}
