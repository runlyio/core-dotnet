using Newtonsoft.Json;
using Runly.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runly
{
	public interface IPackageClient
	{
		Task<IEnumerable<Package>> GetPackages(string organization);
		Task<Package> GetPackage(string organization, string pkgName);
		Task<IEnumerable<PackageVersion>> GetPackageVersions(string organization, string pkgName);
		Task<PackageVersion> GetPackageVersion(string organization, string pkgName, string pkgVersion);
		Task<PackageVersion> UploadPackage(string organization, Stream pkgStream);
		Task<PackageVersion> UploadPackage(string organization, byte[] pkgBytes);

		Task<IEnumerable<JobSearchResult>> SearchJobs(string organization, string query = null);

		Task<JobSchema> GetJobSchema(string organization, Guid jobId);
		Task<TConfig> GetDefaultConfig<TConfig>(string organization, Guid jobId) where TConfig : Config, new();
	}

	public class HttpPackageClient : IPackageClient
	{
		readonly HttpClient api;
		readonly IAuthenticationProvider tokenProvider;

		public HttpPackageClient(HttpClient api, IAuthenticationProvider tokenProvider)
		{
			this.api = api ?? throw new ArgumentNullException(nameof(api));
			this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		}

		public async Task<IEnumerable<Package>> GetPackages(string organization)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/packages");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Package>>();
		}

		public async Task<Package> GetPackage(string organization, string pkgName)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/packages/{pkgName}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Package>();
		}

		public async Task<IEnumerable<PackageVersion>> GetPackageVersions(string organization, string pkgName)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/packages/{pkgName}/versions");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<PackageVersion>>();
		}

		public async Task<PackageVersion> GetPackageVersion(string organization, string pkgName, string pkgVersion)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/packages/{pkgName}/versions/{pkgVersion}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<PackageVersion>();
		}

		public async Task<PackageVersion> UploadPackage(string organization, Stream pkgStream)
		{
			if (pkgStream == null)
				throw new ArgumentNullException(nameof(pkgStream));

			var req = new HttpRequestMessage(HttpMethod.Put, $"/{organization}/packages");
			req.Content = new StreamContent(pkgStream);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<PackageVersion>();
		}

		public async Task<PackageVersion> UploadPackage(string organization, byte[] pkgBytes)
		{
			if (pkgBytes == null)
				throw new ArgumentNullException(nameof(pkgBytes));

			var req = new HttpRequestMessage(HttpMethod.Put, $"/{organization}/packages");
			req.Content = new ByteArrayContent(pkgBytes);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<PackageVersion>();
		}

		public async Task<IEnumerable<JobSearchResult>> SearchJobs(string organization, string query)
		{
			string url = $"/{organization}/jobs/".AddQueryString("q", query);

			var req = new HttpRequestMessage(HttpMethod.Get, url);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<JobSearchResult>>();
		}

		public async Task<JobSchema> GetJobSchema(string organization, Guid jobId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/jobs/{jobId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<JobSchema>();
		}

		public async Task<TConfig> GetDefaultConfig<TConfig>(string organization, Guid jobId)
			where TConfig : Config, new()
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/jobs/{jobId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();

			string json = await response.Content.ReadAsStringAsync();

			var result = JsonConvert.DeserializeAnonymousType(json, new { DefaultConfig = new TConfig() });
			return result?.DefaultConfig;
		}
	}
}
