using System.Collections.Generic;

namespace Runly.Models
{
	public class Pagination<T>
	{
		public bool HasMore { get; set; }

		public IEnumerable<T> Data { get; set; }
	}
}
