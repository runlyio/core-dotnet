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
	/// <summary>
	/// Interface to the Runly.io Run API, allowing clients to enqueue, get, and cancel runs.
	/// </summary>
	public interface IRunClient
	{
		/// <summary>
		/// Gets the runs matching the <paramref name="searchParams">search parameters</paramref>.
		/// </summary>
		/// <param name="organization">The organization to search.</param>
		/// <param name="searchParams">The parameters to use in searching.</param>
		/// <returns>A collection of <see cref="Run">Runs</see></returns>
		Task<Pagination<Run>> Search(string organization, RunSearch searchParams = null);

		/// <summary>
		/// Gets the runs that have not completed successfully, failed, or have been cancelled.
		/// </summary>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to get the active runs for.</param>
		/// <returns>A collection of <see cref="Run">Runs</see></returns>
		Task<IEnumerable<Run>> GetActiveRuns(string organization, string environment);

		/// <summary>
		/// Gets the <see cref="Run"/> with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to get.</param>
		/// <returns>A <see cref="Run"/>.</returns>
		Task<Run> GetRun(string organization, Guid runId);

		/// <summary>
		/// Gets the config for the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to get config for.</param>
		/// <returns>A JSON config.</returns>
		Task<string> GetConfig(string organization, Guid runId);

		/// <summary>
		/// Gets the download URL and size of the logs for the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to get the log info for.</param>
		/// <returns>A URL and size for each log type.</returns>
		Task<IDictionary<RunLogType, RunLogInfo>> GetLogInfo(string organization, Guid runId);

		/// <summary>
		/// Downloads the log for the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to download the log for.</param>
		/// <param name="type">The type of log to download.</param>
		/// <returns>A stream of the requested log.</returns>
		Task<Stream> DownloadLog(string organization, Guid runId, RunLogType type);

		/// <summary>
		/// Enqueues a new run with the <paramref name="config"/> in the <paramref name="organization"/> and <paramref name="environment"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to enqueue a new run in.</param>
		/// <param name="config">The config for the new run.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		Task<Run> Enqueue(string organization, string environment, string config);

		/// <summary>
		/// Enqueues a new run using the config from the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> whose config will be used.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		Task<Run> Requeue(string organization, Guid runId);

		/// <summary>
		/// Requests the cancellation of the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to cancel.</param>
		/// <returns>The <see cref="Run"/> being cancelled.</returns>
		Task<Run> Cancel(string organization, Guid runId);
	}

	/// <summary>
	/// Interface to the Runly.io Run API, allowing clients to enqueue, get, and cancel runs.
	/// </summary>
	public class HttpRunClient : IRunClient
	{
		readonly HttpClient api;
		readonly IAuthenticationProvider tokenProvider;

		/// <summary>
		/// Initializes a new <see cref="HttpRunClient"/>.
		/// </summary>
		public HttpRunClient(HttpClient api, IAuthenticationProvider tokenProvider)
		{
			this.api = api ?? throw new ArgumentNullException(nameof(api));
			this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		}

		/// <summary>
		/// Gets the runs matching the <paramref name="searchParams">search parameters</paramref>.
		/// </summary>
		/// <param name="organization">The organization to search.</param>
		/// <param name="searchParams">The parameters to use in searching.</param>
		/// <returns>A collection of <see cref="Run">Runs</see></returns>
		public async Task<Pagination<Run>> Search(string organization, RunSearch searchParams)
		{
			string url = $"/{organization}/runs".AddQueryString(searchParams?.ToQuerystring());

			var req = new HttpRequestMessage(HttpMethod.Get, url);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Pagination<Run>>();
		}

		/// <summary>
		/// Gets the runs that have not completed successfully, failed, or have been cancelled.
		/// </summary>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to get the active runs for.</param>
		/// <returns>A collection of <see cref="Run">Runs</see></returns>
		public async Task<IEnumerable<Run>> GetActiveRuns(string organization, string environment)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{environment}/runs");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Run>>();
		}

		/// <summary>
		/// Gets the <see cref="Run"/> with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to get.</param>
		/// <returns>A <see cref="Run"/>.</returns>
		public async Task<Run> GetRun(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		/// <summary>
		/// Gets the config for the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to get config for.</param>
		/// <returns>A JSON config.</returns>
		public async Task<string> GetConfig(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/runs/{runId}/config");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsStringAsync();
		}

		/// <summary>
		/// Gets the download URL and size of the logs for the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to get the log info for.</param>
		/// <returns>A URL and size for each log type.</returns>
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

		/// <summary>
		/// Downloads the log for the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to download the log for.</param>
		/// <param name="type">The type of log to download.</param>
		/// <returns>A stream of the requested log.</returns>
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

		/// <summary>
		/// Enqueues a new run with the <paramref name="config"/> in the <paramref name="organization"/> and <paramref name="environment"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to enqueue a new run in.</param>
		/// <param name="config">The config for the new run.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		public async Task<Run> Enqueue(string organization, string environment, string config)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments/{environment}/runs/");
			req.Content = new StringContent(config, Encoding.UTF8, "application/json");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		/// <summary>
		/// Enqueues a new run using the config from the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> whose config will be used.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		public async Task<Run> Requeue(string organization, Guid runId)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/runs/{runId}/");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Run>();
		}

		/// <summary>
		/// Requests the cancellation of the run with ID <paramref name="runId"/>.
		/// </summary>
		/// <param name="organization">The organization in which the <see cref="Run"/> can be found.</param>
		/// <param name="runId">The ID of the <see cref="Run"/> to cancel.</param>
		/// <returns>The <see cref="Run"/> being cancelled.</returns>
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
