using System;
using System.Net.Http;
using System.Threading.Tasks;
using Runly.Models;

namespace Runly
{
	public class ClientTokenProvider : IAuthenticationProvider
	{
		readonly HttpClient runly;
		readonly IAuthentication auth;
		readonly IAccessTokenCache cache;
		readonly string client_id, client_secret, audience;

		public ClientTokenProvider(HttpClient runly, IAuthentication auth, IAccessTokenCache cache, string client_id, string client_secret)
			: this(runly, auth, cache, client_id, client_secret, Audience.Api) { }

		public ClientTokenProvider(HttpClient runly, IAuthentication auth, IAccessTokenCache cache, string client_id, string client_secret, string audience)
		{
			this.runly = runly;
			this.auth = auth;
			this.cache = cache;
			this.client_id = client_id;
			this.client_secret = client_secret;
			this.audience = audience;
		}

		public async Task<string> AcquireToken()
		{
			var token = await cache.GetToken(client_id, client_secret, audience, async () =>
			{
				var info = await GetApiInfo();

				AccessToken t;
				try
				{
					t = await auth.Authenticate(client_id, client_secret, audience, info.Auth.Authority);
				}
				catch (HttpRequestException ex)
				{
					throw new AuthenticationException(ex);
				}

				return t;
			});

			if (String.IsNullOrWhiteSpace(token?.Value))
				return null;

			return token.Value;
		}

		async Task<ApiInfo> GetApiInfo()
		{
			try
			{
				var response = await runly.GetAsync("/");
				await response.EnsureSuccess();
				return await response.Content.ReadAsAsync<ApiInfo>();
			}
			catch (UnsupportedMediaTypeException)
			{
				throw new RunlyApiNotFoundException(runly.BaseAddress.ToString());
			}
			catch (HttpRequestException)
			{
				throw new RunlyApiNotFoundException(runly.BaseAddress.ToString());
			}
		}
	}
}
