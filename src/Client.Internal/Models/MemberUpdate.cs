using Newtonsoft.Json;

namespace Runly.Models
{
	public class MemberUpdate
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<MemberRole>))]
		public Optional<MemberRole> Role { get; set; }
	}
}
