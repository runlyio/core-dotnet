namespace Runly
{
	/// <summary>
	/// The final state of a job run.
	/// </summary>
	public enum Disposition
	{
		NotComplete,
		Successful = 5,
		Failed,
		Cancelled
	}
}
