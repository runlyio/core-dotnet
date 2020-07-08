namespace Runly.Models
{
	public class JobInfo
	{
		/// <summary>
		/// The organization that owns this package.
		/// </summary>
		/// <example>my-org</example>
		public string Org { get; set; }

		/// <summary>
		/// The package name where the job can be found.
		/// </summary>
		/// <example>MyAwesomePackage</example>
		public string Package { get; set; }
		/// <summary>
		/// The package/job version.
		/// </summary>
		/// <example>1.0.0</example>
		public string Version { get; set; }

		/// <summary>
		/// The fully qualified type name of the job.
		/// </summary>
		/// <example>My.Namespace.HelloWorld</example>
		public string Type { get; set; }

		/// <summary>
		/// The short name of the job.
		/// </summary>
		/// <example>HelloWorld</example>
		public string Name { get; set; }
	}
}
