using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Runly
{
	public static class ServiceExtensions
	{
		const string DEFAULT_API_URL = "https://api.runly.io/";

		public static IServiceCollection AddRunlyApi(this IServiceCollection services, string apiKey)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentNullException(nameof(apiKey));

			return AddRunlyApi(services, apiKey, DEFAULT_API_URL);
		}

		public static IServiceCollection AddRunlyApi(this IServiceCollection services, string apiKey, string url)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentNullException(nameof(apiKey));

			if (String.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			return AddRunlyApi(services, _ => apiKey, _ => url);
		}

		public static IServiceCollection AddRunlyApi(this IServiceCollection services, Func<IServiceProvider, string> getApiKey)
			=> AddRunlyApi(services, getApiKey, _ => DEFAULT_API_URL);

		public static IServiceCollection AddRunlyApi(this IServiceCollection services, Func<IServiceProvider, string> getApiKey, Func<IServiceProvider, string> getUrl)
		{
			if (getApiKey == null)
				throw new ArgumentNullException(nameof(getApiKey));

			if (getUrl == null)
				throw new ArgumentNullException(nameof(getUrl));

			services.AddSingleton<IAuthenticationProvider>(s => new ApiKeyProvider(getApiKey(s)));

			// can't use chained AddTypedClient here due to regression in 3.0
			// https://github.com/aspnet/AspNetCore/issues/13346#issuecomment-535544207

			services.AddHttpClient("runly", (s, client) => client.BaseAddress = new Uri(getUrl(s)));

			services.AddTransient<IOrgClient>(s =>
			{
				var factory = s.GetRequiredService<IHttpClientFactory>();
				return new HttpOrgClient(factory.CreateClient("runly"), s.GetRequiredService<IAuthenticationProvider>());
			});

			services.AddTransient<IAccountClient>(s =>
			{
				var factory = s.GetRequiredService<IHttpClientFactory>();
				return new HttpAccountClient(factory.CreateClient("runly"), s.GetRequiredService<IAuthenticationProvider>());
			});

			services.AddTransient<IInfrastructureClient>(s =>
			{
				var factory = s.GetRequiredService<IHttpClientFactory>();
				return new HttpInfrastructureClient(factory.CreateClient("runly"), s.GetRequiredService<IAuthenticationProvider>());
			});

			services.AddTransient<IRunClient>(s =>
			{
				var factory = s.GetRequiredService<IHttpClientFactory>();
				return new HttpRunClient(factory.CreateClient("runly"), s.GetRequiredService<IAuthenticationProvider>());
			});

			return services;
		}
	}
}
