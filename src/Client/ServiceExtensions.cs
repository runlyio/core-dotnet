using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Runly.Client
{
	/// <summary>
	/// Extension methods to add the <see cref="IRunClient"/> to an <see cref="IServiceCollection"/>.
	/// </summary>
	public static class ServiceExtensions
	{
		const string DEFAULT_API_URL = "https://api.runly.io/";

		/// <summary>
		/// Adds the <see cref="IRunClient"/> and related services to the <paramref name="services">IServiceCollection</paramref>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
		/// <param name="apiKey">The key to use when using the Runly API.</param>
		/// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddRunlyApi(this IServiceCollection services, string apiKey)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentNullException(nameof(apiKey));

			return AddRunlyApi(services, apiKey, DEFAULT_API_URL);
		}

		/// <summary>
		/// Adds the <see cref="IRunClient"/> and related services to the <paramref name="services">IServiceCollection</paramref>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
		/// <param name="apiKey">The key to use when using the Runly API.</param>
		/// <param name="url">The URL of the Runly API.</param>
		/// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddRunlyApi(this IServiceCollection services, string apiKey, string url)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentNullException(nameof(apiKey));

			if (String.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			return AddRunlyApi(services, _ => apiKey, _ => url);
		}

		/// <summary>
		/// Adds the <see cref="IRunClient"/> and related services to the <paramref name="services">IServiceCollection</paramref>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
		/// <param name="getApiKey">A function that returns the key to use when using the Runly API.</param>
		/// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
		public static IServiceCollection AddRunlyApi(this IServiceCollection services, Func<IServiceProvider, string> getApiKey)
			=> AddRunlyApi(services, getApiKey, _ => DEFAULT_API_URL);

		/// <summary>
		/// Adds the <see cref="IRunClient"/> and related services to the <paramref name="services">IServiceCollection</paramref>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
		/// <param name="getApiKey">A function that returns the key to use when using the Runly API.</param>
		/// <param name="getUrl">A function that returns the URL of the Runly API.</param>
		/// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
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

			services.AddTransient<IRunClient>(s =>
			{
				var factory = s.GetRequiredService<IHttpClientFactory>();
				return new HttpRunClient(factory.CreateClient("runly"), s.GetRequiredService<IAuthenticationProvider>());
			});

			return services;
		}
	}
}
