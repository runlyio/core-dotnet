using System.Threading.Tasks;

namespace Runly
{
	public interface IAuthentication
	{
		Task<AccessToken> Authenticate(string clientId, string clientSecret, string audience, string authority = null);
	}
}
