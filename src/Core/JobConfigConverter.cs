using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Runly
{
	/// <summary>
	/// Used to deserialize a simplified version of <see cref="JobConfig"/> containing only the type name into the full class.
	/// </summary>
	/// <remarks>
	/// Deserializes a string into the full class. In the following JSON the value "Acme.Job" would be placed in the <see cref="JobConfig.Type"/> property.
	/// ```json
	/// {
	///   "job": "Acme.Job"
	/// }
	/// ```
	/// </remarks>
	public class JobConfigConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(JobConfig);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var cfg = value as JobConfig;

			if (cfg == null)
			{
				writer.WriteNull();
			}
			else
			{
				if (String.IsNullOrWhiteSpace(cfg.Package) && String.IsNullOrWhiteSpace(cfg.Version))
				{
					writer.WriteValue(cfg.Type);
				}
				else
				{
					JToken t = JToken.FromObject(cfg, serializer);
					t.WriteTo(writer);
				}
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return new JobConfig();

			if (reader.TokenType == JsonToken.String)
				return new JobConfig { Type = (string)reader.Value };

			var token = JToken.Load(reader);
			return token.ToObject<JobConfig>();
		}
	}
}
