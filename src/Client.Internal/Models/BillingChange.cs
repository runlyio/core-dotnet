using System;
using Newtonsoft.Json;

namespace Runly.Models
{
	public class BillingChange
	{
		public string PlanName { get; set; }
		public decimal NewTotal { get; set; }
		public decimal DueToday { get; set; }
		public bool IsProrated { get; set; }

		[JsonConverter(typeof(DateWithoutTimeConverter))]
		public DateTime NextBillingDate { get; set; }

		public PaymentMethod PaymentMethod { get; set; }
	}
}
