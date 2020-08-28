using System;
using Newtonsoft.Json;

namespace Runly.Models
{
	public class JobTemplate : JobTemplateSummary
	{
		public dynamic Config { get; set; }
	}

	public class JobTemplateSummary
	{
		public string Id { get; set; }
		public JobConfig Job { get; set; }

		public bool AllowRestrictedApps { get; set; }

		public string Schedule { get; set; }
		public DateTime? NextRunTime { get; set; }

		public string CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }

		public string UpdatedBy { get; set; }
		public DateTime UpdatedAt { get; set; }

		public string LastUsedBy { get; set; }
		public DateTime? LastUsedAt { get; set; }

		public DateTime? LastScheduledRunAt { get; set; }
	}

	public class JobTemplateUpdate
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Id { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<bool>))]
		public Optional<bool> AllowRestrictedApps { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public object Config { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(OptionalJsonConverter<string>))]
		public Optional<string> Schedule { get; set; }
	}
}
