using System;
using System.Collections.Generic;

namespace Runly.Models
{
	public class Environment
	{
		public string Id { get; set; }
		public string Description { get; set; }

		public bool IsProduction { get; set; }

		public IEnumerable<string> Clusters { get; set; }

		public string CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
