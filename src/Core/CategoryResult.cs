namespace Runly
{
	public class CategoryResult
	{
		public string Category { get; private set; }
		public bool IsSuccessful { get; private set; }
		public int Count { get; private set; }

		public CategoryResult(int count, bool isSuccessful, string category)
		{
			this.Count = count;
			this.IsSuccessful = isSuccessful;
			this.Category = category;
		}
	}
}
