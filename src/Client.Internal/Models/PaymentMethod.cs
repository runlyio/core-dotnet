namespace Runly.Models
{
	public class PaymentMethod
	{
		public string CardType { get; set; }
		public string Last4 { get; set; }
		public long ExpMonth { get; set; }
		public long ExpYear { get; set; }
	}
}
