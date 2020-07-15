using System;

namespace Runly
{
	/// <summary>
	/// Error reasons that <see cref="JobInfo"/> may encounter when loading a job.
	/// </summary>
	[Flags]
	public enum JobLoadErrors
	{
		/// <summary>
		/// No errors loading the job.
		/// </summary>
		None = 0,

		/// <summary>
		/// The job type is an interface and cannot be instantiated.
		/// </summary>
		IsInterface = 1,

		/// <summary>
		/// The job type is an abstract class and cannot be instantiated.
		/// </summary>
		IsAbstract = 2,

		/// <summary>
		/// The job type is a generic type definition and cannot be instantiated.
		/// </summary>
		IsGenericTypeDefinition = 4
	}
}
