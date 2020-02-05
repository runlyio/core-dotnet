using Microsoft.Extensions.DependencyInjection;
using System;

namespace Runly.Examples.Census
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddCensusProcesses(this IServiceCollection services)
		{
			services.AddHttpClient<IDownloader, HttpDownloader>((s, client) =>
			{
				var config = s.GetRequiredService<CensusConfig>();
				client.BaseAddress = new Uri(config.BaseUrl);
			});

			services.AddScoped<IDatabase, FakeDatabase>();

			return services;
		}
	}
}
