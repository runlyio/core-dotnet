using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Runly.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Runly.Client
{
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
		/// Enqueues the <see cref="Job"/> described in <paramref name="config"/> to run in the <paramref name="organization"/> 
		/// and <paramref name="environment"/> specified.
		/// </summary>
		/// <param name="client">The API client to use.</param>
		/// <param name="organization">The organization that the job will run in.</param>
		/// <param name="environment">The environment that the job will run in.</param>
		/// <param name="config">The <see cref="Config"/> for the job to run.</param>
		/// <returns>A <see cref="Run"/> indicating the identity and status of the run.</returns>
		public static Task<Run> Enqueue(this IRunClient client, string organization, string environment, Config config) =>
			client.Enqueue(organization, environment, ConfigWriter.ToJson(config));

		public static Task<Run> EnqueueFromTemplate(this IRunClient client, string organization, string environment, string template, object configToMerge, bool scheduled)
		{
			var jobj = JObject.FromObject(configToMerge ?? new { }, ConfigWriter.Serializer);
			jobj["template"] = template;
			jobj["scheduled"] = scheduled;
			string json = jobj.ToString();

			return client.Enqueue(organization, environment, json);
		}

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

		public static async Task<TConfig> GetDefaultConfig<TConfig>(this IPackageClient client, string organization, Guid jobId)
			where TConfig : Config, new()
		{
			string json = await client.GetDefaultConfig(organization, jobId);

			var result = JsonConvert.DeserializeAnonymousType(json, new { DefaultConfig = new TConfig() });
			return result?.DefaultConfig;
		}
	}
}
