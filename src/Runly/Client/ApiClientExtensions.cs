using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Runly.Client.Models;
using System;
using System.Threading.Tasks;

namespace Runly.Client
{
	/// <summary>
	/// Extension methods for the API clients in Runly.Client.
	/// </summary>
	public static class ApiClientExtensions
	{
		/// <summary>
		/// Gets the <see cref="Config"/> for the <paramref name="runId"/> specified.
		/// </summary>
		/// <typeparam name="TConfig">The type of config to get.</typeparam>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization that the run exists in.</param>
		/// <param name="runId">The ID of the run to get the config for.</param>
		/// <returns>A <see cref="Config"/> of type <typeparamref name="TConfig"/>.</returns>
		public static async Task<TConfig> GetConfig<TConfig>(this IRunClient client, string organization, Guid runId) where TConfig : Config =>
			JsonConvert.DeserializeObject<TConfig>(await client.GetConfig(organization, runId), ConfigWriter.Settings);

		/// <summary>
		/// Gets the default <see cref="Config"/> for the <paramref name="jobId"/> specified.
		/// </summary>
		/// <typeparam name="TConfig">The type of <see cref="Config"/>.</typeparam>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization in which the <see cref="Job"/> can be found.</param>
		/// <param name="jobId">The ID of the job to get the default config for.</param>
		/// <returns>A <see cref="Config"/> of type <typeparamref name="TConfig"/>.</returns>
		public static async Task<TConfig> GetDefaultConfig<TConfig>(this IPackageClient client, string organization, Guid jobId)
			where TConfig : Config, new()
		{
			string json = await client.GetDefaultConfig(organization, jobId);

			var result = JsonConvert.DeserializeAnonymousType(json, new { DefaultConfig = new TConfig() });
			return result?.DefaultConfig;
		}

		/// <summary>
		/// Enqueues a new run with the <paramref name="config"/> in the <paramref name="organization"/> and <paramref name="environment"/>.
		/// </summary>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to enqueue a new run in.</param>
		/// <param name="config">The <see cref="Config"/> for the new run.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		public static Task<Run> Enqueue(this IRunClient client, string organization, string environment, Config config) =>
			client.Enqueue(organization, environment, ConfigWriter.ToJson(config));

		/// <summary>
		/// Enqueues a new run with the stored config template in the <paramref name="organization"/> and <paramref name="environment"/>. 
		/// Overrides parameters in the config template with values found on <paramref name="overrides"/>.
		/// </summary>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to enqueue a new run in.</param>
		/// <param name="template">The name of the config template to use for the new run.</param>
		/// <param name="overrides">When provided, will override parameters in the config template.</param>
		/// <param name="scheduled">When true, will count the enqueued run as an occurence of the config template's schedule, affecting the next occurence of the schedule.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		public static Task<Run> EnqueueFromTemplate(this IRunClient client, string organization, string environment, string template, object overrides = null, bool scheduled = false)
		{
			var jobj = JObject.FromObject(overrides ?? new { }, ConfigWriter.Serializer);
			jobj["template"] = template;
			jobj["scheduled"] = scheduled;
			string json = jobj.ToString();

			return client.Enqueue(organization, environment, json);
		}

		/// <summary>
		/// Enqueues a new run with the default config for the <paramref name="jobType"/> in the <paramref name="organization"/> and <paramref name="environment"/>.
		/// </summary>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to enqueue a new run in.</param>
		/// <param name="jobType">The type of job to enqueue a new run for.</param>
		/// <param name="version">The version of the package to use.</param>
		/// <param name="package">The package where the job is found.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		public static Task<Run> EnqueueDefaultConfig(this IRunClient client, string organization, string environment, string jobType, string version = null, string package = null)
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

		/// <summary>
		/// Enqueues a new run with the default config for the <typeparamref name="TJob"/> in the <paramref name="organization"/> and <paramref name="environment"/>.
		/// </summary>
		/// <typeparam name="TJob">The type of job to enqueue a new run for.</typeparam>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization in which the <paramref name="environment"/> can be found.</param>
		/// <param name="environment">The environment to enqueue a new run in.</param>
		/// <param name="version">The version of the package to use.</param>
		/// <param name="package">The package where the job is found.</param>
		/// <returns>A newly enqueued <see cref="Run"/>.</returns>
		public static Task<Run> EnqueueDefaultConfig<TJob>(this IRunClient client, string organization, string environment, string version = null, string package = null)
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
	}
}
