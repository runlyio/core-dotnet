using System;

namespace Runly.Models
{
	public class Payment
	{
		public string Id { get; set; }
		public decimal Amount { get; set; }
		public DateTime CreatedOn { get; set; }
		public string Description { get; set; }
		public bool IsSuccessful { get; set; }
		public PaymentMethod Method { get; set; }
	}
}
