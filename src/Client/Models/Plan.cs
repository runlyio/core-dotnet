using System;
using System.Collections.Generic;

namespace Runly.Models
{
	public class Plan
	{
		public Guid Id { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }

		public decimal Price { get; set; }

		public int? UserCount { get; set; }
		public int? EnvironmentCount { get; set; }
		public int? ClusterCount { get; set; }
		public int? NodeCount { get; set; }
		public int? TemplateCount { get; set; }
		public bool AllowApps { get; set; }
		public IEnumerable<string> Roles { get; set; }
		public IEnumerable<string> Features { get; set; }

		public bool IsFeatured { get; set; }
		public bool IsCurrent { get; set; }
	}
}
