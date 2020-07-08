using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class Member
	{
		public Guid? Id { get; set; }
		public string UserId { get; set; }
		public string Name { get; set; }
		public string Nickname { get; set; }
		public string Email { get; set; }
		public string Picture { get; set; }

		public string InvitedBy { get; set; }
		public DateTime? InvitedOn { get; set; }
		public DateTime? JoinedOn { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MemberRole? Role { get; set; }
	}
}
