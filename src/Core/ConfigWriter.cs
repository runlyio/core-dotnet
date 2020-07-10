using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Runly
{
	/// <summary>
	/// Methods for serializing <see cref="Config">Configs</see>.
	/// </summary>
	public static class ConfigWriter
	{
		/// <summary>
		/// Settings used for the <see cref="JsonSerializer"/>.
		/// </summary>
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

		/// <summary>
		/// A <see cref="JsonSerializer"/> created using the <see cref="Settings"/>.
		/// </summary>
		public static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

		/// <summary>
		/// Serializes a <see cref="Config"/> to JSON.
		/// </summary>
		public static string ToJson(Config config) => JsonConvert.SerializeObject(config, Settings) + Environment.NewLine;

		/// <summary>
		/// Serializes a <see cref="Config"/> to JSON, removing the <see cref="Config.Execution"/> and <see cref="Config.RunlyApi"/> sections for brevity.
		/// </summary>
		public static string ToReducedJson(Config config) => JsonConvert.SerializeObject(ToReducedJObject(config), Settings) + Environment.NewLine;

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
