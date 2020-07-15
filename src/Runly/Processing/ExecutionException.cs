using System;

namespace Runly.Processing
{
	/// <summary>
	/// An exception used internally in <see cref="Execution"/>.
	/// </summary>
	public class ExecutionException : Exception
	{
		/// <summary>
		/// Initializes a new <see cref="ExecutionException"/>..
		/// </summary>
		/// <param name="pointOfFailure">The location where a failure occured.</param>
		/// <param name="innerException">The underlying cause of this exception.</param>
		public ExecutionException(JobMethod pointOfFailure, Exception innerException)
			: base($"Job failed at {pointOfFailure.ToString()}", innerException) { }
	}
}
