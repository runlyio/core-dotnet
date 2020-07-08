using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Runly
{
	public interface IAuthenticationProvider
	{
		Task<string> AcquireToken();
	}

	public static class AuthenticationProviderExtensions
	{
		public static async Task<AuthenticationHeaderValue> AcquireAuthHeader(this IAuthenticationProvider tokenProvider)
		{
			string token = await tokenProvider.AcquireToken();
			return new AuthenticationHeaderValue("Bearer", token);
		}
	}
}
