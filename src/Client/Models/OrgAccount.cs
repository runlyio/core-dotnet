using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class OrgAccount
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string Email { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MemberRole Role { get; set; }

		public bool AllowApps { get; set; }
	}
}
