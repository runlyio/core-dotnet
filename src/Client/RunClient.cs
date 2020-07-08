using Newtonsoft.Json.Linq;
using Runly.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Runly
{
	public interface IRunClient
	{
		Task<Pagination<Run>> Search(string organization, RunSearch searchParams = null);
		Task<IEnumerable<Run>> GetActiveRuns(string organization, string environment);

		Task<Run> GetRun(string organization, Guid runId);
		Task<TConfig> GetConfig<TConfig>(string organization, Guid runId) where TConfig : Config;

		Task<IDictionary<RunLogType, RunLogInfo>> GetLogInfo(string organization, Guid runId);
		Task<Stream> DownloadLog(string organization, Guid runId, RunLogType type);

		Task<Run> Enqueue(string organization, string environment, Config config);
		Task<Run> EnqueueFromTemplate(string organization, string environment, string template, object configToMerge = null, bool scheduled = false);

		Task<Run> Requeue(string organization, Guid runId);
		Task<Run> Cancel(string organization, Guid runId);
	}

	public static class RunsClientExtensions
	{
		public static Task<Run> Enqueue(this IRunClient client, string organization, string environment, string jobType, string version = null, string package = null)
		{
			return client.Enqueue(organization, environment, new Config
			{
				Job = new JobConfig
				{
					Type = jobType,
					Version = version,
					Package = package
				}
			});
		}

		public static Task<Run> Enqueue<TJob>(this IRunClient client, string organization, string environment, string version = null, string package = null)
			where TJob : class
		{
			return client.Enqueue(organization, environment, new Config
			{
				Job = new JobConfig
				{
					Type = typeof(TJob).FullName,
					Version = version,
					Package = package
				}
			});
		}

		public static Task<Run> Enqueue<TConfig>(this IRunClient client, string organization, string environment, string jobType, TConfig config = null, string version = null, string package = null)
			where TConfig : Config
		{
			Config cfg = config ?? new Config();
			cfg.Job = new JobConfig
			{
				Type = jobType,
				Version = version,
				Package = package
			};

			return client.Enqueue(organization, environment, cfg);
		}

		public static Task<Run> Enqueue<TJob, TConfig>(this IRunClient client, string organization, string environment, TConfig config = null, string version = null, string package = null)
			where TJob : class
			where TConfig : Config
		{
			Config cfg = config ?? new Config();
			cfg.Job = new JobConfig
			{
				Type = typeof(TJob).FullName,
				Version = version,
				Package = package
			};

			return client.Enqueue(organization, environment, cfg);
		}
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

		public async Task<TConfig> GetConfig<TConfig>(string organization, Guid runId)
			where TConfig : Config
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}/config");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<TConfig>();
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

		public async Task<Run> Enqueue(string organization, string environment, Config config)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments/{environment}/runs/").WithJsonContent(config);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		public async Task<Run> EnqueueFromTemplate(string organization, string environment, string template, object configToMerge, bool scheduled)
		{
			var jobj = JObject.FromObject(configToMerge ?? new { }, ConfigWriter.Serializer);
			jobj["template"] = template;
			jobj["scheduled"] = scheduled;
			string json = jobj.ToString();

			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments/{environment}/runs/");
			req.Content = new StringContent(json, Encoding.UTF8, "application/json");
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
