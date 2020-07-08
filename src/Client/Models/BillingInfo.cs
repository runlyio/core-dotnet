using System;
using Newtonsoft.Json;

namespace Runly.Models
{
	public class BillingInfo
	{
		public string PlanName { get; set; }
		public decimal PlanPrice { get; set; }

		[JsonConverter(typeof(DateWithoutTimeConverter))]
		public DateTime? NextBillingDate { get; set; }

		public PaymentMethod PaymentMethod { get; set; }
	}
}
