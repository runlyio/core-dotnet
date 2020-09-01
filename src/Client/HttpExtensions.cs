using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Runly.Client
{
	public static class HttpExtensions
	{
		static readonly JsonSerializerSettings settings = new JsonSerializerSettings
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy
				{
					ProcessDictionaryKeys = false
				}
			}
		};

		/// <remarks>
		/// Inspired by http://chocosmith.wordpress.com/2014/05/30/httpresponsemessage-ensuresuccessstatuscode-with-json-result/
		/// </remarks>
		public static async Task EnsureSuccess(this HttpResponseMessage response)
		{
			if (response == null)
				throw new ArgumentNullException("response");

			if (response.IsSuccessStatusCode)
				return;

			string content = null;

			try
			{
				//read the content if we can
				if (response.Content != null)
				{
					content = await response.Content.ReadAsStringAsync();
					if (String.IsNullOrWhiteSpace(content))
						content = null;
				}
			}
			catch
			{
				//swallow the exception, we are about to throw a better exception anyway
			}

			throw new RunlyHttpRequestException(response.StatusCode, response.RequestMessage.Method, response.RequestMessage.RequestUri, content);
		}

		public static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage req, T obj)
		{
			return WithJsonContent<T>(req, obj, "application/json");
		}

		public static HttpRequestMessage WithJsonContent<T>(this HttpRequestMessage req, T obj, string contentType)
		{
			string json = JsonConvert.SerializeObject(obj, settings);
			req.Content = new StringContent(json, Encoding.UTF8, contentType);
			return req;
		}

		public static async Task<T> ReadAsAsync<T>(this HttpContent content)
		{
			if (content?.Headers.ContentType == null)
				return default(T);

			if (content.Headers.ContentType.MediaType != "application/json")
				throw new UnsupportedMediaTypeException($"Cannot parse content type '{content.Headers.ContentType.MediaType}' as JSON.");

			string json = await content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(json, settings);
		}
	}

	public class UnsupportedMediaTypeException : Exception
	{
		public UnsupportedMediaTypeException(string message)
			: base(message) { }

		public UnsupportedMediaTypeException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
