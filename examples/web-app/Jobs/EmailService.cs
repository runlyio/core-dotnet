using System.Threading.Tasks;

namespace Examples.WebApp.Jobs
{
	public interface IEmailService
	{
		Task SendEmail(string email, string subject, string message);
	}

	public class FakeEmailService : IEmailService
	{
		public FakeEmailService(string apiKey)
		{
			// a real email service would do something with the API key
		}

		public Task SendEmail(string email, string subject, string message)
		{
			// let's pretend it's a slow-ass email service
			return Task.Delay(1500);
		}
	}
}
