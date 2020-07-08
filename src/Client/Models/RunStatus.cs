using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class RunCountStatus
	{
		public Guid Id { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public RunState State { get; set; }

		public DateTime? StartedAt { get; set; }
		public ProgressInfo Progress { get; set; }

		public class ProgressInfo
		{
			public int? Total { get; set; }
		}
	}

	public class RunProgressStatus
	{
		public Guid Id { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public RunState State { get; set; }

		public DateTime? StartedAt { get; set; }
		public ProgressInfo Progress { get; set; }

		public class ProgressInfo
		{
			public int Success { get; set; }
			public int Failed { get; set; }
			public IEnumerable<ItemProgress> Categories { get; set; }
		}
	}
}
