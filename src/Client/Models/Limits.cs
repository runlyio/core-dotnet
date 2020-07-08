using System.Collections.Generic;

namespace Runly.Models
{
	public class Limits
	{
		public int? UserCount { get; set; }
		public int? EnvironmentCount { get; set; }
		public int? ClusterCount { get; set; }
		public int? NodeCount { get; set; }

		public IEnumerable<RoleItem> Roles { get; set; }

		public class RoleItem
		{
			public string Role { get; set; }
			public bool IsAvailable { get; set; }
		}
	}
}
