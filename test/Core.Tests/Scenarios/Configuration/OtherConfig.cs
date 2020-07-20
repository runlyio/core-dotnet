using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace Runly.Tests.Scenarios.Configuration
{
	public class OtherConfig
	{
		[Required]
		public JobConfig Job { get; set; }

		public class JobConfig
		{
			[NotNull]
			public string Package { get; set; }

			[NotNull]
			public string Version { get; set; }

			[Required]
			public string Type { get; set; }
		}
	}
}
