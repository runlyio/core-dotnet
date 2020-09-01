using Newtonsoft.Json.Linq;
using Runly.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Runly.Client
{
	public interface IRunClient
	{
		Task<Pagination<Run>> Search(string organization, RunSearch searchParams = null);
		Task<IEnumerable<Run>> GetActiveRuns(string organization, string environment);
		Task<Run> GetRun(string organization, Guid runId);
		Task<string> GetConfig(string organization, Guid runId);
		Task<IDictionary<RunLogType, RunLogInfo>> GetLogInfo(string organization, Guid runId);
		Task<Stream> DownloadLog(string organization, Guid runId, RunLogType type);
		Task<Run> Enqueue(string organization, string environment, string config);
		Task<Run> Requeue(string organization, Guid runId);
		Task<Run> Cancel(string organization, Guid runId);
	}

	public class HttpRunClient : IRunClient
	{
		readonly HttpClient api;
		readonly IAuthenticationProvider tokenProvider;

		public HttpRunClient(HttpClient api, IAuthenticationProvider tokenProvider)
		{
			this.api = api ?? throw new ArgumentNullException(nameof(api));
			this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		}

		public async Task<Pagination<Run>> Search(string organization, RunSearch searchParams)
		{
			string url = $"/{organization}/runs".AddQueryString(searchParams?.ToQuerystring());

			var req = new HttpRequestMessage(HttpMethod.Get, url);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Pagination<Run>>();
		}

		public async Task<IEnumerable<Run>> GetActiveRuns(string organization, string environment)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{environment}/runs");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Run>>();
		}

		public async Task<Run> GetRun(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		public async Task<string> GetConfig(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}/config");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<string>();
		}

		public async Task<IDictionary<RunLogType, RunLogInfo>> GetLogInfo(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}/logs");

			req.Headers.Accept.Clear();
			req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IDictionary<RunLogType, RunLogInfo>>();
		}

		public async Task<Stream> DownloadLog(string organization, Guid runId, RunLogType type)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}/logs/{type}");

			req.Headers.Accept.Clear();
			req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsStreamAsync();
		}

		public async Task<Run> Enqueue(string organization, string environment, string config)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments/{environment}/runs/");
			req.Content = new StringContent(config, Encoding.UTF8, "application/json");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		public async Task<Run> Requeue(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/runs/{runId}/");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		public async Task<Run> Cancel(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/runs/{runId}/cancel");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}
	}
}
