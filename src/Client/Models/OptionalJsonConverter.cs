using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Runly.Models
{
	public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
	{
		public override bool CanRead => true;

		public override void WriteJson(JsonWriter writer, Optional<T> value, JsonSerializer serializer)
		{
			if (value.HasValue)
			{
				if (value.Value is string)
				{
					writer.WriteValue(value.Value);
				}
				else if (value.Value is Enum)
				{
					writer.WriteValue(value.Value.ToString());
				}
				else if (value.Value is IEnumerable arr)
				{
					writer.WriteStartArray();
					foreach (var item in arr)
					{
						writer.WriteValue(item);
					}
					writer.WriteEndArray();
				}
				else
				{
					writer.WriteValue(value.Value);
				}
			}
		}

		public override Optional<T> ReadJson(JsonReader reader, Type objectType, Optional<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			// need to implement the ReadJson since Newtonsoft will not use the implicit
			// operator if the json value is null and instead just throw an exception

			if (reader.TokenType == JsonToken.StartArray)
			{
				JToken token = JToken.Load(reader);
				var items = token.ToObject<T>();
				return new Optional<T>((T)items);
			}

			if (reader.TokenType == JsonToken.String && typeof(T).IsEnum)
			{
				T value;
				try
				{
					value = (T)Enum.Parse(typeof(T), (string)reader.Value);
				}
				catch (ArgumentException ex)
				{
					throw new JsonSerializationException($"Invalid value: {(string)reader.Value}", ex);
				}

				return new Optional<T>(value);
			}

			return new Optional<T>((T)reader.Value);
		}
	}
}