using System;

namespace Runly.Models
{
	public class Application
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string SecretKey { get; set; }
		public string PublishableKey { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime? LastUsedAt { get; set; }
	}
}
