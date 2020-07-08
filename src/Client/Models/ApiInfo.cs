namespace Runly.Models
{
	public class ApiInfo
	{
		public string Version { get; set; }
		public AuthInfo Auth { get; set; }
		public string StripeApiKey { get; set; }
	}

	public class AuthInfo
	{
		public string Authority { get; set; }
		public string Audience { get; set; }
	}
}