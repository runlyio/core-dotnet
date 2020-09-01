using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Runly.Hosting;

namespace Runly
{
	/// <summary>
	/// Provides convenience methods for creating instances of <see cref="IHostBuilder"/> with pre-configured defaults.
	/// </summary>
	public static class JobHost
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HostBuilder"/> class with pre-configured defaults.
		/// </summary>
		/// <remarks>
		///   The following defaults are applied to the returned <see cref="HostBuilder"/>:
		///   <list type="bullet">
		///     <item><description>set the <see cref="IHostEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/></description></item>
		///     <item><description>load host <see cref="IConfiguration"/> from "DOTNET_" prefixed environment variables</description></item>
		///     <item><description>load host <see cref="IConfiguration"/> from supplied command line args</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from User Secrets when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development' using the entry assembly</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from environment variables</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from supplied command line args</description></item>
		///     <item><description>configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output</description></item>
		///     <item><description>enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'</description></item>
		///   </list>
		/// </remarks>
		/// <param name="args">The command line args.</param>
		/// <returns>The initialized <see cref="IHostBuilder"/>.</returns>
		public static IHostBuilder CreateDefaultBuilder(string[] args)
		{
			return CreateDefaultBuilder(args, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HostBuilder"/> class with pre-configured defaults.
		/// </summary>
		/// <remarks>
		///   The following defaults are applied to the returned <see cref="HostBuilder"/>:
		///   <list type="bullet">
		///     <item><description>set the <see cref="IHostEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/></description></item>
		///     <item><description>load host <see cref="IConfiguration"/> from "DOTNET_" prefixed environment variables</description></item>
		///     <item><description>load host <see cref="IConfiguration"/> from supplied command line args</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from User Secrets when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development' using the entry assembly</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from environment variables</description></item>
		///     <item><description>load app <see cref="IConfiguration"/> from supplied command line args</description></item>
		///     <item><description>configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output</description></item>
		///     <item><description>enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'</description></item>
		///   </list>
		/// </remarks>
		/// <param name="args">The command line args.</param>
		/// <param name="jobAssemblies">The assemblies in which to search for <see cref="IJob"/>s.</param>
		/// <returns>The initialized <see cref="IHostBuilder"/>.</returns>
		public static IHostBuilder CreateDefaultBuilder(string[] args, params Assembly[] jobAssemblies)
		{
			return Host
				.CreateDefaultBuilder()
				.ConfigureServices(services => services.AddRunlyJobs(args, jobAssemblies));
		}

		/// <summary>
		/// Runs the job.
		/// </summary>
		/// <param name="host">The <see cref="IHost"/> to run.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public static Task RunJobAsync(this IHost host)
		{
			return host.RunJobAsync(new CancellationTokenSource().Token);
		}

		/// <summary>
		/// Runs the job.
		/// </summary>
		/// <param name="host">The <see cref="IHost"/> to run.</param>
		/// <param name="cancellationToken">The token to trigger cancellation.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public static Task RunJobAsync(this IHost host, CancellationToken cancellationToken)
		{
			var action = host.Services.GetService<IHostAction>();
			return action?.RunAsync(cancellationToken) ?? Task.CompletedTask;
		}
	}
}
