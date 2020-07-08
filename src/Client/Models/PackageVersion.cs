using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class PackageVersion
	{
		public string Id { get; set; }
		public string Version { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public PackageType Type { get; set; }

		public string ClientVersion { get; set; }

		public DateTime UploadedAt { get; set; }
		public string UploadedBy { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public PackageVersionStatus Status { get; set; }
		public string ErrorMessage { get; set; }

		public IEnumerable<Job> Jobs { get; set; } = new Job[0];
	}
}
