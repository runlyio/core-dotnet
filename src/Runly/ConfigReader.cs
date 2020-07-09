using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Runly
{
	public class ConfigReader
	{
		readonly JobCache cache;

		public ConfigReader(JobCache cache)
		{
			this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}

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

		private bool IsReducedForm(JObject job)
		{
			return GetJob(job).Type == JTokenType.String;
		}

		private JToken GetJob(JObject job) => job.GetValue(nameof(Job), StringComparison.OrdinalIgnoreCase);
	}
}
