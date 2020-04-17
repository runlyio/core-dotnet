using Microsoft.Extensions.Logging;
using Runly;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Examples.GettingStarted.Census
{
	public interface IDownloader
	{
		Task<Stream> Download(string name);
	}

	public class HttpDownloader : IDownloader
	{
		readonly HttpClient client;
		readonly ILogger<HttpDownloader> logger;

		public HttpDownloader(HttpClient client, ILogger<HttpDownloader> logger)
		{
			this.client = client;
			this.logger = logger;
		}

		public async Task<Stream> Download(string name)
		{
			logger.LogInformation("Downloading {name} from {host}", name, client.BaseAddress.Host);

			var response = await client.GetAsync(name);
			await response.EnsureSuccess();

			return await response.Content.ReadAsStreamAsync();
		}
	}
}
