using Runly.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runly
{
	public interface IInfrastructureClient
	{
		Task<Node> GetNode(string org, Guid nodeId);
		Task<IEnumerable<Node>> GetNodes(string org, string cluster);

		Task<IEnumerable<Cluster>> GetClusters(string organization);
		Task<IEnumerable<Cluster>> GetClusters(string organization, string environment);
		Task<Cluster> GetCluster(string organization, string id);
		Task<Cluster> CreateCluster(string organization, string id, IEnumerable<string> environments);
		Task DeleteCluster(string organization, string id);

		Task AddClustersToEnvironment(string organization, string environment, IEnumerable<string> clusters);
		Task RemoveClusterFromEnvironment(string organization, string environment, string cluster);
		Task ClearClustersFromEnvironment(string organization, string environment);

		Task<IEnumerable<Models.Environment>> GetEnvironments(string organization);
		Task<Models.Environment> GetEnvironment(string organization, string id);
		Task<Models.Environment> CreateEnvironment(string organization, string id, bool isProduction = false, string description = null, IEnumerable<string> clusters = null);
		Task<Models.Environment> UpdateEnvironment(string organization, string id, EnvironmentUpdate data);
		Task DeleteEnvironment(string organization, string id);

		Task<IEnumerable<EnvironmentPackage>> GetEnvironmentPackages(string organization, string id);
		Task<EnvironmentPackage> GetEnvironmentPackage(string organization, string environment, string package);
		Task<EnvironmentPackage> UpdateEnvironmentPackage(string organization, string environment, string package, EnvironmentPackageUpdate data);

		Task<IEnumerable<JobTemplateSummary>> GetJobTemplates(string organization, string environment);
		Task<JobTemplate> GetJobTemplate(string organization, string environment, string id);
		Task<JobTemplate> CreateJobTemplate(string organization, string environment, string id, bool allowRestrictedApps, object config, JobConfig job, string schedule = null);
		Task<JobTemplate> UpdateJobTemplate(string organization, string environment, string id, JobTemplateUpdate template);
		Task DeleteJobTemplate(string organization, string environment, string id);
	}

	public static class InfrastructureClientExtensions
	{
		public static Task<Models.Environment> CreateEnvironment(this IInfrastructureClient client, string organization, string id, params string[] clusters)
		{
			return client.CreateEnvironment(organization, id, false, null, (IEnumerable<string>)clusters);
		}

		public static Task<Models.Environment> CreateEnvironment(this IInfrastructureClient client, string organization, string id, bool isProduction, string description, params string[] clusters)
		{
			return client.CreateEnvironment(organization, id, isProduction, description, (IEnumerable<string>)clusters);
		}

		public static Task<Cluster> CreateCluster(this IInfrastructureClient client, string organization, string id, params string[] environments)
		{
			return client.CreateCluster(organization, id, (IEnumerable<string>)environments);
		}

		public static Task<Models.Environment> UpdateEnvironmentId(this IInfrastructureClient client, string organization, string existingId, string newId)
		{
			return client.UpdateEnvironment(organization, existingId, new EnvironmentUpdate
			{
				Id = newId
			});
		}

		public static Task<Models.Environment> UpdateEnvironmentDescription(this IInfrastructureClient client, string organization, string id, string description)
		{
			return client.UpdateEnvironment(organization, id, new EnvironmentUpdate
			{
				Description = description
			});
		}

		public static Task<Models.Environment> UpdateEnvironmentProductionFlag(this IInfrastructureClient client, string organization, string id, bool isProduction)
		{
			return client.UpdateEnvironment(organization, id, new EnvironmentUpdate
			{
				IsProduction = isProduction
			});
		}

		public static Task<EnvironmentPackage> UpdateEnvironmentPackageVersionRange(this IInfrastructureClient client, string organization, string environment, string package, string versionRange)
		{
			return client.UpdateEnvironmentPackage(organization, environment, package, new EnvironmentPackageUpdate
			{
				VersionRange = versionRange
			});
		}

		public static Task AddClustersToEnvironment(this IInfrastructureClient client, string organization, string environment, params string[] clusters)
		{
			return client.AddClustersToEnvironment(organization, environment, (IEnumerable<string>)clusters);
		}

		public static Task<JobTemplate> CreateJobTemplate<TJob>(this IInfrastructureClient client, string organization, string environment, string id, bool allowRestrictedApps, object config, string package = null, string version = null, string schedule = null)
			where TJob : class
		{
			return client.CreateJobTemplate(organization, environment, id, allowRestrictedApps, config, new JobConfig
			{
				Type = typeof(TJob).FullName,
				Package = package,
				Version = version
			}, schedule);
		}

		public static Task<JobTemplate> UpdateJobTemplateId(this IInfrastructureClient client, string organization, string environment, string oldId, string newId)
		{
			return client.UpdateJobTemplate(organization, environment, oldId, new JobTemplateUpdate
			{
				Id = newId
			});
		}

		public static Task<JobTemplate> UpdateJobTemplateAppRestriction(this IInfrastructureClient client, string organization, string environment, string id, bool allowRestrictedApps)
		{
			return client.UpdateJobTemplate(organization, environment, id, new JobTemplateUpdate
			{
				AllowRestrictedApps = allowRestrictedApps
			});
		}

		public static Task<JobTemplate> UpdateJobTemplateConfig(this IInfrastructureClient client, string organization, string environment, string id, object config)
		{
			return client.UpdateJobTemplate(organization, environment, id, new JobTemplateUpdate
			{
				Config = config
			});
		}

		public static Task<JobTemplate> UpdateJobTemplateSchedule(this IInfrastructureClient client, string organization, string environment, string id, string schedule)
		{
			return client.UpdateJobTemplate(organization, environment, id, new JobTemplateUpdate
			{
				Schedule = schedule
			});
		}
	}

	public class HttpInfrastructureClient : IInfrastructureClient
	{
		readonly HttpClient api;
		readonly IAuthenticationProvider tokenProvider;

		public HttpInfrastructureClient(HttpClient api, IAuthenticationProvider tokenProvider)
		{
			this.api = api ?? throw new ArgumentNullException(nameof(api));
			this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		}

		public async Task<Node> GetNode(string org, Guid nodeId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/nodes/{nodeId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Node>();
		}

		public async Task<IEnumerable<Node>> GetNodes(string org, string cluster)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/clusters/{cluster}/nodes");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Node>>();
		}

		public async Task<IEnumerable<Cluster>> GetClusters(string organization)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/clusters");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Cluster>>();
		}

		public async Task<IEnumerable<Cluster>> GetClusters(string organization, string environment)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{environment}/clusters");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Cluster>>();
		}

		public async Task<Cluster> GetCluster(string organization, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/clusters/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Cluster>();
		}

		public async Task<Cluster> CreateCluster(string organization, string id, IEnumerable<string> environments)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/clusters").WithJsonContent(new { id, environments });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Cluster>();
		}

		public async Task DeleteCluster(string organization, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/{organization}/clusters/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return;

			await response.EnsureSuccess();
		}

		public async Task AddClustersToEnvironment(string organization, string environment, IEnumerable<string> clusters)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments/{environment}/clusters").WithJsonContent(new { clusters });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
		}

		public async Task RemoveClusterFromEnvironment(string organization, string environment, string cluster)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/{organization}/environments/{environment}/clusters/{cluster}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
		}

		public async Task ClearClustersFromEnvironment(string organization, string environment)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/{organization}/environments/{environment}/clusters");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
		}

		public async Task<IEnumerable<Models.Environment>> GetEnvironments(string organization)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Models.Environment>>();
		}

		public async Task<Models.Environment> GetEnvironment(string organization, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Models.Environment>();
		}

		public async Task<Models.Environment> CreateEnvironment(string organization, string id, bool isProduction, string description, IEnumerable<string> clusters)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments").WithJsonContent(new { id, description, isProduction, clusters });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Models.Environment>();
		}

		public async Task<Models.Environment> UpdateEnvironment(string organization, string id, EnvironmentUpdate data)
		{
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/{organization}/environments/{id}").WithJsonContent(data);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Models.Environment>();
		}

		public async Task DeleteEnvironment(string organization, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/{organization}/environments/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return;

			await response.EnsureSuccess();
		}

		public async Task<IEnumerable<EnvironmentPackage>> GetEnvironmentPackages(string organization, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{id}/packages");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<EnvironmentPackage>>();
		}

		public async Task<EnvironmentPackage> GetEnvironmentPackage(string organization, string environment, string package)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{environment}/packages/{package}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<EnvironmentPackage>();
		}

		public async Task<EnvironmentPackage> UpdateEnvironmentPackage(string organization, string environment, string package, EnvironmentPackageUpdate data)
		{
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/{organization}/environments/{environment}/packages/{package}").WithJsonContent(data);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<EnvironmentPackage>();
		}

		public async Task<IEnumerable<JobTemplateSummary>> GetJobTemplates(string organization, string environment)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{environment}/templates/");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<JobTemplateSummary>>();
		}

		public async Task<JobTemplate> GetJobTemplate(string organization, string environment, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{organization}/environments/{environment}/templates/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<JobTemplate>();
		}

		public async Task<JobTemplate> CreateJobTemplate(string organization, string environment, string id, bool allowRestrictedApps, object config, JobConfig job, string schedule)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{organization}/environments/{environment}/templates/").WithJsonContent(new
			{
				id,
				allowRestrictedApps,
				job,
				config,
				schedule
			});
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<JobTemplate>();
		}

		public async Task<JobTemplate> UpdateJobTemplate(string organization, string environment, string id, JobTemplateUpdate template)
		{
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/{organization}/environments/{environment}/templates/{id}").WithJsonContent(template);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<JobTemplate>();
		}

		public async Task DeleteJobTemplate(string organization, string environment, string id)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/{organization}/environments/{environment}/templates/{id}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
		}
	}
}
