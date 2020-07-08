namespace Runly
{
	public class ItemResultSummary
	{
		public int Count { get; private set; }
		public bool IsSuccessful { get; private set; }
		public string Category { get; private set; }

		public ItemResultSummary(int count, bool isSuccessful, string category)
		{
			this.Count = count;
			this.IsSuccessful = isSuccessful;
			this.Category = category;
		}
	}
}
