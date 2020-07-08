using System.Collections.Generic;
using Newtonsoft.Json;

namespace Runly.Models
{
	public class EnvironmentUpdate
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Id { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Description { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<bool>))]
		public Optional<bool> IsProduction { get; set; }
	}
}
