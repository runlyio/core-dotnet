namespace Runly
{
	/// <summary>
	/// The state of <see cref="Execution"/>.
	/// </summary>
	public enum ExecutionState
	{
		/// <summary>
		/// Indicates the execution has not started.
		/// </summary>
		NotStarted,

		/// <summary>
		/// Indicates the execution is calling InitializeAsync.
		/// </summary>
		Initializing,

		/// <summary>
		/// Indicates the execution is calling GetItemsAsync and preparing to enumerate the items.
		/// </summary>
		GettingItemsToProcess,

		/// <summary>
		/// Indicates the execution is calling ProcessAsync with the items.
		/// </summary>
		Processing,

		/// <summary>
		/// Indicates the execution is calling FinalizeAsync.
		/// </summary>
		Finalizing,

		/// <summary>
		/// Indicates the execution has completed.
		/// </summary>
		Complete
	}
}
