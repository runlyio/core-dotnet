using CommandLine;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runly.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Runly
{
	public static class ServiceExtensions
	{
		const int INVALID_ARGS = -1;

		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, Config config)
		{
			return services.AddRunlyJobs(config, Assembly.GetCallingAssembly());
		}

		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, Config config, params Assembly[] jobAssemblies)
		{
			services.AddJobCache(jobAssemblies);

			services.AddRunAction(config);

			return services;
		}

		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, string[] args)
		{
			return services.AddRunlyJobs(args, Assembly.GetCallingAssembly());
		}

		public static IServiceCollection AddRunlyJobs(this IServiceCollection services, string[] args, params Assembly[] jobAssemblies)
		{
			var cfgReader = services.AddJobCache(jobAssemblies);

			var parseResults = Parser.Default.ParseArguments<ListVerb, GetVerb, RunVerb>(args)
				.WithParsed<ListVerb>(services.AddListAction)
				.WithParsed<GetVerb>(services.AddGetAction)
				.WithParsed<RunVerb>(v => services.AddRunAction(v, cfgReader))
				.WithNotParsed(errors => Environment.Exit(INVALID_ARGS));

			return services;
		}

		static ConfigReader AddJobCache(this IServiceCollection services, IEnumerable<Assembly> jobAssemblies)
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

			return cfgReader;
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

		static void AddRunAction(this IServiceCollection services, RunVerb verb, ConfigReader cfgReader)
		{
			if (verb.Debug)
				services.AddSingleton(new Debug() { AttachDebugger = true });

			var config = cfgReader.FromFile(verb.ConfigPath);

			config.__filePath = verb.ConfigPath;

			string dir = Path.GetDirectoryName(verb.ConfigPath);
			if (!String.IsNullOrWhiteSpace(dir))
				Directory.SetCurrentDirectory(dir);

			if (!string.IsNullOrWhiteSpace(verb.ResultsPath))
			{
				config.Execution.ResultsToFile = true;
				config.Execution.ResultsFilePath = verb.ResultsPath;
			}

			if (verb.Silent)
				config.Execution.ResultsToConsole = false;

			services.AddRunAction(config);
		}

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
