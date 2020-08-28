using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Runly.Models
{
	public class Package
	{
		public string Id { get; set; }
		public IEnumerable<string> Versions { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public PackageType Type { get; set; }

		public DateTime UpdatedAt { get; set; }
		public string UpdatedBy { get; set; }

		public bool IsPublic { get; set; }
	}
}
