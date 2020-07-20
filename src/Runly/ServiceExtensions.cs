using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Runly.Internal;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

			var root = new RootCommand();

			var run = new Command("run", "Runs a job.");
			run.Handler = CommandHandler.Create<string, string>((run, job) =>
			{

			});
			root.AddCommand(run);

			foreach (var job in cache.Jobs)
			{
				var jobCmd = new Command(job.JobType.Name);
				jobCmd.AddAlias(job.JobType.Name.ToLowerInvariant());
				jobCmd.AddAlias(job.JobType.FullName);
				jobCmd.AddAlias(job.JobType.FullName.ToLowerInvariant());
				AddConfigParams(jobCmd, job.ConfigType, null);
				jobCmd.Handler = CommandHandler.Create<int>(n =>
				{
					
				});
				run.AddCommand(jobCmd);
			}

			void AddConfigParams(Command command, Type type, string prefix)
			{
				foreach (var prop in type.GetProperties())
				{
					var name = prefix == null ? $"--{prop.Name}" : $"{prefix}.{prop.Name}";

					if (prop.PropertyType.IsValueType)
					{
						if (prop.CanWrite)
						{
							var option = new Option(name)
							{
								Argument = new Argument()
								{
									Arity = prop.PropertyType == typeof(bool) ? ArgumentArity.ZeroOrOne : ArgumentArity.ExactlyOne,
									ArgumentType = prop.PropertyType
								}
							};
							option.AddAlias(name.ToLowerInvariant());
							command.AddOption(option);
						}
					}
					else
					{
						if (!prop.PropertyType.IsArray)
							AddConfigParams(command, prop.PropertyType, name);
					}
				}
			}

			new CommandLineBuilder(root)
				.UseMiddleware(async (context, next) =>
				{
					var runCmdResult = context.ParseResult.RootCommandResult.Children["run"];

					if (runCmdResult != null)
					{
						var jobCmdResult = runCmdResult.Children.FirstOrDefault() as CommandResult;

						if (jobCmdResult != null)
						{
							var config = cache.GetDefaultConfig(jobCmdResult.Command.Name);

							ApplyOverrides(config, jobCmdResult.Children);

							AddRunAction(services, config);
						}
					}
					else
					{
						await next(context);
					}
				})
				.Build()
				.Invoke(args);

			//var parseResults = Parser.Default.ParseArguments<ListVerb, GetVerb, RunVerb>(args)
			//	.WithParsed<ListVerb>(services.AddListAction)
			//	.WithParsed<GetVerb>(services.AddGetAction)
			//	.WithParsed<RunVerb>(v => services.AddRunAction(v, cache, cfgReader))
			//	.WithNotParsed(errors => Environment.Exit(INVALID_ARGS));

			return services;
		}

		/// <summary>
		/// Applies command line config overrides to <paramref name="config"/>.
		/// </summary>
		/// <param name="config">The <see cref="Config"/> to override.</param>
		/// <param name="symbols">A list of overrides to apply to the config.</param>
		static void ApplyOverrides(Config config, SymbolResultSet symbols)
		{
			foreach (var symbol in symbols)
			{
				var optr = symbol as OptionResult;

				var path = optr.Option.Name;

				var argr = optr.Children.FirstOrDefault() as ArgumentResult;

				var arg = argr.GetValueOrDefault();

				//if (parts.Length != 2)
				//	throw new FormatException($"Config override '{string.Join(' ', parts)}' must be in the format '--property value'.");

				var prop = path.Split('.');

				object cfg = null;
				PropertyInfo pi = null;
				var type = config.GetType();

				for (int i = 0; i < prop.Length; i++)
				{
					cfg = pi?.GetValue(cfg) ?? config;
					pi = type.GetProperty(prop[i]);

					if (pi == null)
						pi = type.GetProperties().SingleOrDefault(p => p.Name.Equals(prop[i], StringComparison.InvariantCultureIgnoreCase));

					if (pi == null)
						throw new ArgumentException($"Could not find '{prop[i]}' in the config path '{path}'");

					type = pi.PropertyType;
				}

				//if (type != typeof(string))
				//{
				//	var converter = TypeDescriptor.GetConverter(type);

				//	if (converter == null)
				//		throw new ArgumentException($"Could not find a type converter for the type '{type.FullName}' for the config path '{path}'.");

				//	try
				//	{
				//		arg = converter.ConvertFromInvariantString(arg);
				//	}
				//	catch (NotSupportedException ex)
				//	{
				//		throw new ArgumentException($"Could not convert '{arg}' to the type '{type.FullName}' for the config path '{path}'.", ex);
				//	}
				//}

				pi.SetValue(cfg, arg);
			}
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
