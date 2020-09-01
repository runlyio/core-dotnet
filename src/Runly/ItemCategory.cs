namespace Runly
{
	/// <summary>
	/// The summary of <see cref="ItemResult">ItemResults</see> for a given category.
	/// </summary>
	public class ItemCategory
	{
		internal int count;

		/// <summary>
		/// Indicates whether the results in this category were successfully processed.
		/// </summary>
		public bool IsSuccessful { get; set; }

		/// <summary>
		/// Gets the name of the category.
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets the number of items in this category of results.
		/// </summary>
		public int Count => count;

		/// <summary>
		/// Initializes a new <see cref="ItemCategory"/>.
		/// </summary>
		public ItemCategory(int count, bool isSuccessful, string category)
		{
			this.count = count;
			this.IsSuccessful = isSuccessful;
			this.Category = category;
		}
	}
}
