using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runly.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Runly
{
	/// <summary>
	/// Extension methods for <see cref="IServiceCollection"/> to add services for Runly jobs.
	/// </summary>
	public static class ServiceExtensions
	{
		const int INVALID_ARGS = -1;

		/// <summary>
		/// Adds the services required to execute the job specified in <paramref name="config"/>. The job type must be found in the calling assembly.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="config">The <see cref="Config"/> that determines the job that will be executed.</param>
		/// <returns>The same <see cref="IServiceCollection"/> passed in for chaining.</returns>
		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, Config config)
		{
			return services.AddRunlyJobs(config, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Adds the services required to execute the job specified in <paramref name="config"/>. The job type must be found in the <paramref name="jobAssemblies"/>.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="config">The <see cref="Config"/> that determines the job that will be executed.</param>
		/// <param name="jobAssemblies">The assemblies where the job specified in <paramref name="config"/> can be found.</param>
		/// <returns>The same <see cref="IServiceCollection"/> passed in for chaining.</returns>
		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, Config config, params Assembly[] jobAssemblies)
		{
			services.AddJobCache(jobAssemblies);

			services.AddRunAction(config);

			return services;
		}

		/// <summary>
		/// Adds the services required to perform the action specified in <paramref name="args"/> given the jobs found in the calling assembly.
		/// </summary>
		/// <param name="args">The command line arguments that determine what action the <see cref="JobHost"/> will perform.</param>
		/// <param name="services">The service collection being modified.</param>
		/// <returns>The same <see cref="IServiceCollection"/> passed in for chaining.</returns>
		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, string[] args)
		{
			return services.AddRunlyJobs(args, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Adds the services required to perform the action specified in <paramref name="args"/> given the jobs found in the <paramref name="jobAssemblies"/>.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="args">The command line arguments that determine what action the <see cref="JobHost"/> will perform.</param>
		/// <param name="jobAssemblies">The assemblies where jobs can be found.</param>
		/// <returns>The same <see cref="IServiceCollection"/> passed in for chaining.</returns>
		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, string[] args, params Assembly[] jobAssemblies)
		{
			(var cache, var cfgReader) = services.AddJobCache(jobAssemblies);

			var parseResults = Parser.Default.ParseArguments<ListVerb, GetVerb, RunVerb>(args)
				.WithParsed<ListVerb>(services.AddListAction)
				.WithParsed<GetVerb>(services.AddGetAction)
				.WithParsed<RunVerb>(v => services.AddRunAction(v, cache, cfgReader))
				.WithNotParsed(errors => Environment.Exit(INVALID_ARGS));

			return services;
		}

		/// <summary>
		/// Discovers and caches job types from the <paramref name="jobAssemblies"/>, creating a singleton service <see cref="JobCache"/>.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="jobAssemblies">The assemblies where jobs can be found.</param>
		/// <returns>A <see cref="ConfigReader"/> that can read JSON configs for the job types in the <see cref="JobCache"/>.</returns>
		static (JobCache, ConfigReader) AddJobCache(this IServiceCollection services, IEnumerable<Assembly> jobAssemblies)
		{
			// need to use this stuff now; create it then register it
			var cache = new JobCache(jobAssemblies);
			var cfgReader = new ConfigReader(cache);

			services.AddSingleton(cache);
			services.AddSingleton(cfgReader);

			foreach (var job in cache.Jobs)
			{
				if (job.IsValid)
				{
					services.AddTransient(job.JobType);
				}
			}

			return (cache, cfgReader);
		}

		static void AddListAction(this IServiceCollection services, ListVerb verb)
		{
			services.AddTransient<JsonSchema>();

			services.AddTransient<IHostAction>(s => new ListAction(
				verb.Verbose,
				verb.Json,
				s.GetRequiredService<JobCache>(),
				s.GetRequiredService<JsonSchema>()
			));
		}

		static void AddGetAction(this IServiceCollection services, GetVerb verb)
		{
			services.AddTransient<IHostAction>(s => new GetAction(
				verb.Verbose,
				verb.Type,
				string.IsNullOrWhiteSpace(verb.FilePath) ? null : verb.FilePath,
				s.GetRequiredService<JobCache>()
			));
		}

		/// <summary>
		/// Adds a transient <see cref="RunAction"/> to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="verb">The command line verb to that specifies the location of the config and other modifiers.</param>
		/// <param name="cfgReader">A <see cref="ConfigReader"/> that can read the JSON config file specified in <paramref name="verb"/>.</param>
		static void AddRunAction(this IServiceCollection services, RunVerb verb, JobCache cache, ConfigReader cfgReader)
		{
			if (verb.Debug)
				services.AddSingleton(new Debug() { AttachDebugger = true });

			Config config;

			if (File.Exists(verb.JobOrConfigPath))
			{
				config = cfgReader.FromFile(verb.JobOrConfigPath);

				config.__filePath = verb.JobOrConfigPath;

				string dir = Path.GetDirectoryName(verb.JobOrConfigPath);
				if (!String.IsNullOrWhiteSpace(dir))
					Directory.SetCurrentDirectory(dir);
			}
			else
			{
				// TODO: Catch TypeNotFoundException and output message saying
				// the path or job type specified could not be found.
				config = cache.GetDefaultConfig(verb.JobOrConfigPath);
			}

			cfgReader.ApplyOverrides(config, verb.Props);

			if (verb.Silent)
				config.Execution.ResultsToConsole = false;

			services.AddRunAction(config);
		}

		/// <summary>
		/// Adds a transient <see cref="RunAction"/> to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="config">The <see cref="Config"/> for the job.</param>
		static void AddRunAction(this IServiceCollection services, Config config)
		{
			services.AddConfig(config);

			services.AddSingleton<TextWriter>(s =>
			{
				if (!config.Execution.ResultsToFile)
					return new VoidTextWriter();

				if (string.IsNullOrWhiteSpace(config.Execution.ResultsFilePath))
					config.Execution.ResultsFilePath = "results.json";

				return new StreamWriter(File.Open(config.Execution.ResultsFilePath, FileMode.Create));
			});

			services.AddSingleton<TextWriter>(s =>
			{
				if (!config.Execution.ResultsToConsole)
					return new VoidTextWriter();

				return Console.Out;
			});

			if (!String.IsNullOrWhiteSpace(config.RunlyApi?.Uri) && !String.IsNullOrWhiteSpace(config.RunlyApi?.Token))
			{
				services.AddRunlyApi(config.RunlyApi.Token, config.RunlyApi.Uri);

				services.AddSingleton(s => new ResultsChannel(
					new Uri(config.RunlyApi.Uri),
					s.GetRequiredService<IAuthenticationProvider>(),
					s.GetRequiredService<ILogger<ResultsChannel>>()
				));
			}

			services.AddSingleton<Execution>(s =>
			{
				var cache = s.GetRequiredService<JobCache>();

				var info = cache.Get(config.Job.Type);

				var job = (IJob)s.GetRequiredService(info.JobType);
				return job.GetExecution(s.GetRequiredService<IServiceProvider>());
			});

			services.AddTransient<IHostAction>(s => new RunAction(
				s.GetRequiredService<Execution>(),
				s.GetRequiredService<Config>(),
				s.GetRequiredService<ILogger<RunAction>>(),
				s.GetService<ResultsChannel>(),
				s.GetService<Debug>()
			));
		}

		/// <summary>
		/// Adds the <paramref name="config"/> to the <see cref="IServiceCollection"/> as a singleton service using the class itself, each base class, and each interface.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="config">The <see cref="Config"/> to add to the service collection.</param>
		/// <returns>The same <see cref="IServiceCollection"/> passed in for chaining.</returns>
		public static IServiceCollection AddConfig(this IServiceCollection services, Config config)
		{
			var type = config.GetType();

			foreach (var intf in type.GetInterfaces())
				services.AddSingleton(intf, config);

			do
			{
				services.AddSingleton(type, config);
				type = type.BaseType;
			} while (type != null);

			return services;
		}
	}
}
