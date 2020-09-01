using Newtonsoft.Json;
using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Runly
{
	/// <summary>
	/// The base config class used to configure jobs.
	/// </summary>
	/// <remarks>Public members of this class and classes that extend it must be JSON serializable.</remarks>
	public class Config
	{
		[JsonIgnore]
		internal string __filePath;

		/// <summary>
		/// Identifies the job that created this config and/or the job that will run when executed.
		/// </summary>
		[Required]
		[JsonConverter(typeof(JobConfigConverter))]
		public JobConfig Job { get; set; }

		/// <summary>
		/// Determines how the job will execute.
		/// </summary>
		[NotNull]
		public ExecutionConfig Execution { get; set; }

		/// <summary>
		/// Runly API connection information used to report progress and store results of the job.
		/// </summary>
		[JsonSchemaIgnore]
		public RunlyApiConfig RunlyApi { get; set; }

		/// <summary>
		/// Initializes a new <see cref="Config"/>.
		/// </summary>
		public Config()
		{
			Job = new JobConfig();
			Execution = new ExecutionConfig();
			RunlyApi = new RunlyApiConfig();
		}
	}
}
