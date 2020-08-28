namespace Runly.Models
{
	public class JobSchema
	{
		/// <summary>
		/// The default config for the job.
		/// </summary>
		public object DefaultConfig { get; set; }

		/// <summary>
		/// The JSON schema for the config.
		/// </summary>
		public object Schema { get; set; }
	}
}
