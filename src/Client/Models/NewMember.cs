using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class NewMember
	{
		public string Email { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public MemberRole Role { get; set; }
	}
}
