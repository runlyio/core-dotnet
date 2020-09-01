using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Runly
{
	/// <summary>
	/// Identifies the job that created the <see cref="Config"/> and/or the job that will run when executed.
	/// </summary>
	/// <remarks>The data in this class is used by the Runly API, node and/or job host. This class should not be extended.</remarks>
	public class JobConfig
	{
		/// <summary>
		/// The ID of the package that contains the <see cref="Type"/>.
		/// </summary>
		[NotNull]
		public string Package { get; set; }

		/// <summary>
		/// The version of the package that contains the <see cref="Type"/>.
		/// </summary>
		/// <value>A valid semver.org version in the format MAJOR.MINOR.PATCH.</value>
		[NotNull]
		public string Version { get; set; }

		/// <summary>
		/// The full name of the type of job.
		/// </summary>
		[Required]
		public string Type { get; set; }
	}
}
