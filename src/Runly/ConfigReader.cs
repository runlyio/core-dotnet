using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Runly.Hosting;

namespace Runly
{
	/// <summary>
	/// Deserializes JSON config files into instances of <see cref="Config"/>.
	/// </summary>
	public class ConfigReader
	{
		readonly JobCache cache;

		/// <summary>
		/// Initializes a new <see cref="ConfigReader"/>.
		/// </summary>
		/// <param name="cache">The <see cref="JobCache"/> containing the config types that can be read.</param>
		public ConfigReader(JobCache cache)
		{
			this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}

		/// <summary>
		/// Creates an instance of a <see cref="Config"/> from the JSON file found at <paramref name="path"/>.
		/// </summary>
		/// <param name="path">A file path contianing a JSON config.</param>
		/// <returns>An instance of <see cref="Config"/> from the deserialized file.</returns>
		public Config FromFile(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			if (!File.Exists(path))
				throw new FileNotFoundException("File not found", path);

			string configJson = null;

			using (var reader = new StreamReader(path))
			{
				configJson = reader.ReadToEnd();
			}

			return FromJson(configJson);
		}

		/// <summary>
		/// Creates an instance of a <see cref="Config"/> from the JSON string.
		/// </summary>
		/// <param name="json">A string containing a JSON config.</param>
		/// <returns>An instance of <see cref="Config"/> from the deserialized string.</returns>
		public Config FromJson(string json)
		{
			var jobj = JObject.Parse(json);
			string type;

			if (IsReducedForm(jobj))
			{
				// json is reduced form config, so get then remove 'Job' before deserializing
				var job = GetJob(jobj);
				type = job.ToString();

				jobj.Remove(job.Path);
			}
			else
			{
				type = GetJob(jobj).ToObject<JobConfig>().Type;
			}

			var jobInfo = cache.Get(type);

			if (jobInfo == null)
				throw new ConfigException($"Could not find job type {type}.", nameof(Config.Job));

			if (!typeof(Config).IsAssignableFrom(jobInfo.ConfigType))
				throw new ArgumentException($"{jobInfo.ConfigType.FullName} does not extend {typeof(Config).FullName}.", nameof(jobInfo.ConfigType));

			var config = jobj.ToObject(jobInfo.ConfigType) as Config;

			config.Job.Type = jobInfo.JobType.FullName;

			return config;
		}

		private bool IsReducedForm(JObject job) => GetJob(job).Type == JTokenType.String;

		private JToken GetJob(JObject job) => job.GetValue(nameof(Job), StringComparison.OrdinalIgnoreCase);
	}
}
