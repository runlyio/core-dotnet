using System;
using System.Collections.Generic;

namespace Runly.Models
{
	public class Cluster
	{
		public string Id { get; set; }
		public string ApiKey { get; set; }

		public NodeCountInfo Nodes { get; set; }

		public DateTime? StatusChangedAt { get; set; }
		public bool IsOnline { get; set; }

		public string CreatedBy { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public IEnumerable<EnvironmentInfo> Environments { get; set; }

		public class NodeCountInfo
		{
			public int Online { get; set; }
			public int Disabled { get; set; }
		}
	}
}
