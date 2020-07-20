using System;
using Microsoft.Extensions.DependencyInjection;

namespace Runly
{
	/// <summary>
	/// Extension methods for <see cref="IServiceCollection"/> to add services for Runly jobs with config.
	/// </summary>
	public static class ConfigServiceExtensions
	{
		/// <summary>
		/// Adds a transient service of the type specified in <paramref name="serviceType"/> with a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="serviceType">The type of the service to register.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Transient"/>
		public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Func<IServiceProvider, Config, object> implementationFactory)
		{
			return services.AddTransient(serviceType, s => implementationFactory(s, s.GetRequiredService<Config>()));
		}

		/// <summary>
		/// Adds a transient service of the type specified in <typeparamref name="TService"/> with a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TConfig">The type of <see cref="Config" /> that the job uses.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Transient"/>
		public static IServiceCollection AddTransient<TService, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TService> implementationFactory)
			where TService : class
			where TConfig : Config
		{
			return services.AddTransient<TService>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		/// <summary>
		/// Adds a transient service of the type specified in <typeparamref name="TService"/> with an
		/// implementation type specified in <typeparamref name="TImplementation"/> and a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
		/// <typeparam name="TConfig">The type of <see cref="Config" /> that the job uses.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Transient"/>
		public static IServiceCollection AddTransient<TService, TImplementation, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TImplementation> implementationFactory)
			where TService : class
			where TImplementation : class, TService
			where TConfig : Config
		{
			return services.AddTransient<TService, TImplementation>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		/// <summary>
		/// Adds a scoped service of the type specified in <paramref name="serviceType"/> with a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="serviceType">The type of the service to register.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Scoped"/>
		public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Func<IServiceProvider, Config, object> implementationFactory)
		{
			return services.AddScoped(serviceType, s => implementationFactory(s, s.GetRequiredService<Config>()));
		}

		/// <summary>
		/// Adds a scoped service of the type specified in <typeparamref name="TService"/> with a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TConfig">The type of <see cref="Config" /> that the job uses.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Scoped"/>
		public static IServiceCollection AddScoped<TService, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TService> implementationFactory)
			where TService : class
			where TConfig : Config
		{
			return services.AddScoped<TService>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		/// <summary>
		/// Adds a scoped service of the type specified in <typeparamref name="TService"/> with an
		/// implementation type specified in <typeparamref name="TImplementation"/> and a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
		/// <typeparam name="TConfig">The type of <see cref="Config" /> that the job uses.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Scoped"/>
		public static IServiceCollection AddScoped<TService, TImplementation, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TImplementation> implementationFactory)
			where TService : class
			where TImplementation : class, TService
			where TConfig : Config
		{
			return services.AddScoped<TService, TImplementation>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		/// <summary>
		/// Adds a singleton service of the type specified in <paramref name="serviceType"/> with a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="serviceType">The type of the service to register.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Singleton"/>
		public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Func<IServiceProvider, Config, object> implementationFactory)
		{
			return services.AddSingleton(serviceType, s => implementationFactory(s, s.GetRequiredService<Config>()));
		}

		/// <summary>
		/// Adds a singleton service of the type specified in <typeparamref name="TService"/> with a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TConfig">The type of <see cref="Config" /> that the job uses.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Singleton"/>
		public static IServiceCollection AddSingleton<TService, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TService> implementationFactory)
			where TService : class
			where TConfig : Config
		{
			return services.AddSingleton<TService>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}

		/// <summary>
		/// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an
		/// implementation type specified in <typeparamref name="TImplementation"/> and a
		/// factory specified in <paramref name="implementationFactory"/> to the
		/// specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <typeparam name="TService">The type of the service to add.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
		/// <typeparam name="TConfig">The type of <see cref="Config" /> that the job uses.</typeparam>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
		/// <param name="implementationFactory">The factory that creates the service that takes the job's config as a parameter.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		/// <seealso cref="ServiceLifetime.Singleton"/>
		public static IServiceCollection AddSingleton<TService, TImplementation, TConfig>(this IServiceCollection services, Func<IServiceProvider, TConfig, TImplementation> implementationFactory)
			where TService : class
			where TImplementation : class, TService
			where TConfig : Config
		{
			return services.AddSingleton<TService, TImplementation>(s => implementationFactory(s, s.GetRequiredService<TConfig>()));
		}
	}
}
