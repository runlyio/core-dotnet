using System;
using Microsoft.Extensions.DependencyInjection;

namespace Runly
{
	public static class ConfigServiceExtensions
	{
		public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Func<IServiceProvider, Config, object> implementationFactory)
		{
			return services.AddTransient(serviceType, s => implementationFactory(s, s.GetRequiredService<Config>()));
		}

		public static IServiceCollection AddTransient<TService, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TService> implementationFactory)
			where TService : class
			where TConfig : Config
		{
			return services.AddTransient<TService>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		public static IServiceCollection AddTransient<TService, TImplementation, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TImplementation> implementationFactory)
			where TService : class
			where TImplementation : class, TService
			where TConfig : Config
		{
			return services.AddTransient<TService, TImplementation>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Func<IServiceProvider, Config, object> implementationFactory)
		{
			return services.AddScoped(serviceType, s => implementationFactory(s, s.GetRequiredService<Config>()));
		}

		public static IServiceCollection AddScoped<TService, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TService> implementationFactory)
			where TService : class
			where TConfig : Config
		{
			return services.AddScoped<TService>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		public static IServiceCollection AddScoped<TService, TImplementation, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TImplementation> implementationFactory)
			where TService : class
			where TImplementation : class, TService
			where TConfig : Config
		{
			return services.AddScoped<TService, TImplementation>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Func<IServiceProvider, Config, object> implementationFactory)
		{
			return services.AddSingleton(serviceType, s => implementationFactory(s, s.GetRequiredService<Config>()));
		}

		public static IServiceCollection AddSingleton<TService, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TService> implementationFactory)
			where TService : class
			where TConfig : Config
		{
			return services.AddSingleton<TService>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		public static IServiceCollection AddSingleton<TService, TImplementation, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TImplementation> implementationFactory)
			where TService : class
			where TImplementation : class, TService
			where TConfig : Config
		{
			return services.AddSingleton<TService, TImplementation>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}
	}
}
