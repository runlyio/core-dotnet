namespace Runly
{
	/// <summary>
	/// A list of methods found on a Job.
	/// </summary>
	public enum JobMethod
	{
		/// <summary>
		/// The <see cref="IJob.InitializeAsync"/> method.
		/// </summary>
		InitializeAsync,

		/// <summary>
		/// The GetItemsAsync method.
		/// </summary>
		GetItemsAsync,

		/// <summary>
		/// The Count operation performed on an enumerable list of items.
		/// </summary>
		Count,

		/// <summary>
		/// The GetEnumeratorAsync method on an enumerable list of items.
		/// </summary>
		GetEnumerator,

		/// <summary>
		/// The MoveNextAsync method on IAsyncEnumerator.
		/// </summary>
		EnumeratorMoveNext,

		/// <summary>
		/// The Current property on IAsyncEnumerator.
		/// </summary>
		EnumeratorCurrent,

		/// <summary>
		/// The GetItemIdAsync method.
		/// </summary>
		GetItemIdAsync,

		/// <summary>
		/// TheProcessAsync method.
		/// </summary>
		ProcessAsync,

		/// <summary>
		/// The <see cref="IJob.FinalizeAsync(Disposition)"/> method.
		/// </summary>
		FinalizeAsync
	}
}
