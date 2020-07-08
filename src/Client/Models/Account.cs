using System;

namespace Runly.Models
{
	public class Account
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Nickname { get; set; }
		public string Email { get; set; }
		public string Picture { get; set; }

		public DateTime? JoinedAt { get; set; }

		public bool CanEdit { get; set; }
	}
}
