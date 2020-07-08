using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class Event
	{
		public string Type { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ItemId { get; set; }

		public bool IsSuccessful { get; set; }
		public Error Error { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ExitCode { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? Pid { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string User { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(StringEnumConverter))]
		public RunState? Disposition { get; set; }

		public DateTime Time { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? Duration { get; set; }

		public override string ToString() => !String.IsNullOrWhiteSpace(ItemId) ? $"{ItemId} ({Type})" : Type;
	}
}
