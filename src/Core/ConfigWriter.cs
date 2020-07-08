using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Runly
{
	public static class ConfigWriter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy
				{
					ProcessDictionaryKeys = false
				}
			},
			Formatting = Formatting.Indented
		};

		public static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

		public static string ToJson(Config config)
		{
			return JsonConvert.SerializeObject(config, Settings) + Environment.NewLine;
		}

		public static string ToReducedJson(Config config)
		{
			return JsonConvert.SerializeObject(ToReducedJObject(config), Settings) + Environment.NewLine;
		}

		internal static JObject ToReducedJObject(Config config)
		{
			config.Job.Version = null;
			config.Job.Package = null;

			var job = JObject.FromObject(config, Serializer);

			job.Remove(nameof(Config.RunlyApi).ToCamelCase());
			job.Remove(nameof(Config.Execution).ToCamelCase());

			return job;
		}
	}
}
