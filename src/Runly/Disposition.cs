namespace Runly
{
	/// <summary>
	/// The final state of a job run.
	/// </summary>
	public enum Disposition
	{
		/// <summary>
		/// Indicates the run has not completed, so there is no <see cref="Disposition"/> yet.
		/// </summary>
		NotComplete,

		/// <summary>
		/// Indicates the run completed successfully.
		/// </summary>
		Successful = 5,

		/// <summary>
		/// Indicates the run exceeded the item failure threshold and stopped running, terminating in a failed state.
		/// </summary>
		Failed,

		/// <summary>
		/// Indicates the run was cancelled by a user and stopped running.
		/// </summary>
		Cancelled
	}
}
