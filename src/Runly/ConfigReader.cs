using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Runly
{
	/// <summary>
	/// Deserializes JSON config files into instances of <see cref="Config"/>.
	/// </summary>
	public  class ConfigReader
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

		/// <summary>
		/// Applies command line config overrides to <paramref name="config"/>.
		/// </summary>
		/// <param name="config">The <see cref="Config"/> to override.</param>
		/// <param name="overrides">A list of overrides in the format 'property=value'.</param>
		internal void ApplyOverrides(Config config, IEnumerable<string> overrides)
		{
			foreach (var ovr in overrides)
			{
				if (!ovr.Contains('='))
					throw new FormatException($"Config override '{ovr}' must be in the format 'property=value'.");

				var parts = ovr.Split('=');

				if (parts.Length != 2)
					throw new FormatException($"Config override '{ovr}' must be in the format 'property=value'.");

				var prop = parts[0].Split('.');

				object cfg = null;
				PropertyInfo pi = null;
				var type = config.GetType();

				for (int i = 0; i < prop.Length; i++)
				{
					cfg = pi?.GetValue(cfg) ?? config;
					pi = type.GetProperty(prop[i]);

					if (pi == null)
						throw new ArgumentException($"Could not find '{prop[i]}' in the config path '{parts[0]}'");
					
					type = pi.PropertyType;
				}

				object val = parts[1];

				if (type != typeof(string))
				{
					var converter = TypeDescriptor.GetConverter(type);

					if (converter == null)
						throw new ArgumentException($"Could not find a type converter for the type '{type.FullName}' for the config path '{parts[0]}'.");

					try
					{
						val = converter.ConvertFromInvariantString(parts[1]);
					}
					catch (NotSupportedException ex)
					{
						throw new ArgumentException($"Could not convert the string '{parts[1]}' to the type '{type.FullName}' for the config path '{parts[0]}'.", ex);
					}
				}

				pi.SetValue(cfg, val);
			}
		}
	}
}
