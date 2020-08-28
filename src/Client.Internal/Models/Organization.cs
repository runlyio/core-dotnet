using System;

namespace Runly.Models
{
	public class Organization
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string DefaultEnvironment { get; set; }

		public string Plan { get; set; }
		public Limits Limits { get; set; }

		public string TimeZone { get; set; }

		public string CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
