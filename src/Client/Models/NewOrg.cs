using System;

namespace Runly.Models
{
	public class NewOrg
	{
		public string Name { get; set; }
		public string TimeZone { get; set; }
		public Guid? PlanId { get; set; }
		public string PaymentMethodId { get; set; }
	}
}
