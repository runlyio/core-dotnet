using System;
using System.Collections.Generic;

namespace Runly.Client.Models
{
	public class JobSearchResult
	{
		public string Name { get; set; }
		public string Type { get; set; }

		public IEnumerable<JobSummary> Versions { get; set; }

		public class JobSummary
		{
			public Guid Id { get; set; }
			public string ConfigType { get; set; }
			public string Version { get; set; }
		}
	}
}
