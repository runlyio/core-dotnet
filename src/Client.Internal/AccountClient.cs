using Runly.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runly
{
	public interface IAccountClient
	{
		Task<Account> GetAccount();
		Task<Account> UpdateAccount(AccountUpdate acct);
		Task<IEnumerable<OrgAccount>> GetOrganizationAccounts();
		Task<OrgAccount> UpdateOrganizationAccount(string org, string email);

		Task<IEnumerable<Application>> GetApplications();
		Task<Application> AddApplication(string name);
		Task RevokeApplication(int id);
	}

	public class HttpAccountClient : IAccountClient
	{
		readonly HttpClient api;
		readonly IAuthenticationProvider tokenProvider;

		public HttpAccountClient(HttpClient api, IAuthenticationProvider tokenProvider)
		{
			this.api = api ?? throw new ArgumentNullException(nameof(api));
			this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		}

		public async Task<Account> GetAccount()
		{
			var req = new HttpRequestMessage(HttpMethod.Get, "/account/");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Account>();
		}

		public async Task<Account> UpdateAccount(AccountUpdate acct)
		{
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/account/").WithJsonContent(acct);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Account>();
		}

		public async Task<IEnumerable<OrgAccount>> GetOrganizationAccounts()
		{
			var req = new HttpRequestMessage(HttpMethod.Get, "/account/orgs/");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<OrgAccount>>();
		}

		public async Task<OrgAccount> UpdateOrganizationAccount(string org, string email)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/account/orgs/{org}").WithJsonContent(new { email });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<OrgAccount>();
		}

		public async Task<IEnumerable<Application>> GetApplications()
		{
			var req = new HttpRequestMessage(HttpMethod.Get, "/apps/");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Application>>();
		}

		public async Task<Application> AddApplication(string name)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, "/apps/").WithJsonContent(new { name });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Application>();
		}

		public async Task RevokeApplication(int id)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/apps/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
		}
	}
}
