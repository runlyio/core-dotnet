using System.Collections.Generic;

namespace Runly.Models
{
	public class Usage
	{
		public UsageLimits Member { get; set; }
		public UsageLimits Environment { get; set; }
		public UsageLimits Cluster { get; set; }

		public IEnumerable<TemplateLimits> Templates { get; set; }

		public class UsageLimits
		{
			public int? Limit { get; set; }
			public int Current { get; set; }
		}

		public class TemplateLimits : UsageLimits
		{
			public string Environment { get; set; }
		}
	}
}
