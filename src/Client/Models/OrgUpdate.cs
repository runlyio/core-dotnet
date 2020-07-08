using Newtonsoft.Json;

namespace Runly.Models
{
	public class OrgUpdate
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Id { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Name { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> TimeZone { get; set; }
	}
}
