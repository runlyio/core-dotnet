using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runly
{
	public class Auth0Authentication : IAuthentication
	{
		readonly HttpClient client;

		public Auth0Authentication(HttpClient client)
		{
			this.client = client;
		}

		public async Task<AccessToken> Authenticate(string client_id, string client_secret, string audience, string authority)
		{
			const string url = "/oauth/token";

			var req = !String.IsNullOrWhiteSpace(authority)
				? new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(authority), url))
				: new HttpRequestMessage(HttpMethod.Post, url);

			req = req.WithJsonContent(new
			{
				grant_type = "client_credentials",
				audience,
				client_id,
				client_secret
			});

			var response = await client.SendAsync(req);
			await response.EnsureSuccess();

			return await response.Content.ReadAsAsync<AccessToken>();
		}
	}
}
