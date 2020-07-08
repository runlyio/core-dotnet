using System;

namespace Runly.Processing
{
	public class ExecutionException : Exception
	{
		public ExecutionException(JobMethod pointOfFailure, Exception innerException)
			: base($"Job failed at {pointOfFailure.ToString()}", innerException) { }
	}
}
