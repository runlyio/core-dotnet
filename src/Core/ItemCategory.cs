namespace Runly
{
	public class ItemCategory
	{
		internal int count;

		public bool IsSuccessful { get; set; }
		public string Category { get; set; }
		public int Count => count;
	}
}
