using Newtonsoft.Json;

namespace Runly.Models
{
	public class AccountUpdate
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Name { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Email { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Password { get; set; }
	}
}
