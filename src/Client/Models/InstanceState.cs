namespace Runly.Models
{
	public enum InstanceState
	{
		NotStarted,
		Initializing,
		GettingItemsToProcess,
		Processing,
		Finalizing,
		Successful,
		Failed,
		Cancelled,
		TimedOut,
		Error
	}
}
