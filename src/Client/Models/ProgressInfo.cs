using System.Collections.Generic;

namespace Runly.Models
{
	public class ProgressInfo
	{
		public int Success { get; set; }
		public int Failed { get; set; }
		public int? Total { get; set; }

		public IEnumerable<ItemProgress> Categories { get; set; } = new ItemProgress[0];
	}
}
