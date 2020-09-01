namespace Runly.Client.Models
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
