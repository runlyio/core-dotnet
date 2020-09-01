using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runly.Client;
using Runly.Hosting;
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
		const string ListCommand = "list";
		const string GetCommand = "get";
		const string RunCommand = "run";
		const string ConfigPathArgument = "configPath";

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
			(var cache, var reader) = services.AddJobCache(jobAssemblies);

			GetCommandLineBuilder(services, cache, reader)
			.UseMiddleware(async (context, next) =>
			{
				var result = context.ParseResult.RootCommandResult.Children.FirstOrDefault() as CommandResult;

				if (result?.Command.Name == ListCommand || result?.Command.Name == GetCommand || result?.Command.Name == RunCommand ||  context.ParseResult.UnmatchedTokens.Count > 0)
				{
					await next(context);
				}
				else if (result != null) // Job command
				{
					var config = cache.GetDefaultConfig(result.Command.Name);

					ApplyCommandLineOverrides(config, result.Children);

					services.AddRunAction(config);
				}
				else
				{
					await next(context);
				}
			})
			.UseParseErrorReporting()
			.Build()
			.Invoke(args);

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

		static CommandLineBuilder GetCommandLineBuilder(IServiceCollection services, JobCache cache, ConfigReader reader)
		{
			var root = new RootCommand();

			var list = new Command(ListCommand, "Lists the jobs available to run.")
			{
				new Option<bool>(new[] { "--verbose", "-v" }, "Optional. Changes the config serialization to include all properties."),
				new Option<bool>(new[] { "--json", "-j" }, "Optional. Changes the output format to JSON.")
			};
			list.Handler = CommandHandler.Create<bool, bool>((verbose, json) => services.AddListAction(verbose, json));
			root.AddCommand(list);

			var get = new Command(GetCommand, "Writes the default config for the specified type to the file path.")
			{
				new Argument<string>("type", "The type of job to get the default config for."),
				new Argument<string>("filePath", () => "", "Optional. The file path to write the config to."),
				new Option<bool>(new[] { "--verbose", "-v" }, "Optional. Changes the config serialization to include all properties.")
			};
			get.Handler = CommandHandler.Create<string, string, bool>((type, filePath, verbose) => services.AddGetAction(type, filePath, verbose));
			root.AddCommand(get);

			var run = new Command(RunCommand, "Runs a job from a JSON config file.")
			{
				new Argument<FileInfo>(ConfigPathArgument, "The JSON serialized config for the job to run.").ExistingOnly(),
				new Argument<FileInfo>("resultsPath", () => null, "Optional. The file path to write the results of the job to."),
				new Option<bool>(new[] { "--debug", "-d" }, "Optional. Prompts the user to attach a debugger when the job starts."),
				new Option<bool>(new[] { "--silent", "-s" }, "Optional. Silences console output.")
			};
			run.Handler = CommandHandler.Create<FileInfo, FileInfo, bool, bool>((configPath, resultsPath, debug, silent) => services.AddRunAction(reader, configPath, resultsPath, debug, silent));
			root.AddCommand(run);

			// Add a command for each job, with each config path as an option
			foreach (var job in cache.Jobs)
			{
				var jobCmd = new Command(job.JobType.Name.ToLowerInvariant(), $"Runs the job {job.JobType.FullName}.");

				jobCmd.AddAlias(job.JobType.Name);

				AddConfigOptions(jobCmd, job.ConfigType, null);

				root.AddCommand(jobCmd);
			}

			void AddConfigOptions(Command command, Type type, string prefix)
			{
				foreach (var prop in type.GetProperties())
				{
					var name = prefix == null ? $"--{prop.Name}" : $"{prefix}.{prop.Name}";

					// The job type comes from the command and the package/version cannot be set (it's whatever's executing)
					if (name == "--Job")
						continue;

					bool isArray = prop.PropertyType.IsArray;
					Type propType = isArray ? prop.PropertyType.GetElementType() : prop.PropertyType;

					if (propType.IsValueType || propType == typeof(string))
					{
						if (prop.CanWrite)
						{
							var desc = prop.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;

							var option = new Option(name.ToLowerInvariant(), desc)
							{
								Argument = new Argument(propType.Name)
								{
									Arity = (propType == typeof(bool), isArray) switch
									{
										(true, false) => ArgumentArity.ZeroOrOne,
										(_, true) => ArgumentArity.ZeroOrMore,
										(_, _) => ArgumentArity.ExactlyOne
									},
									ArgumentType = prop.PropertyType
								}
							};
							option.AddAlias(name);
							command.AddOption(option);
						}
					}
					else
					{
						if (!prop.PropertyType.IsArray)
							AddConfigOptions(command, prop.PropertyType, name);
					}
				}
			}

			return new CommandLineBuilder(root);
		}

		/// <summary>
		/// Applies command line config overrides to <paramref name="config"/>.
		/// </summary>
		/// <param name="config">The <see cref="Config"/> to override.</param>
		/// <param name="symbols">A list of overrides to apply to the config.</param>
		static void ApplyCommandLineOverrides(Config config, SymbolResultSet symbols)
		{
			foreach (var symbol in symbols)
			{
				var optr = symbol as OptionResult;

				var path = optr.Option.Name;
				var argr = optr.Children.FirstOrDefault() as ArgumentResult;
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

				pi.SetValue(cfg, argr.GetValueOrDefault());
			}
		}

		static void AddListAction(this IServiceCollection services, bool verbose, bool json)
		{
			services.AddTransient<JsonSchema>();

			services.AddTransient<IHostAction>(s => new ListAction(
				verbose,
				json,
				s.GetRequiredService<JobCache>(),
				s.GetRequiredService<JsonSchema>()
			));
		}

		static void AddGetAction(this IServiceCollection services, string type, string filePath, bool verbose)
		{
			services.AddTransient<IHostAction>(s => new GetAction(
				verbose,
				type,
				string.IsNullOrWhiteSpace(filePath) ? null : filePath,
				s.GetRequiredService<JobCache>()
			));
		}

		/// <summary>
		/// Adds a transient <see cref="RunAction"/> to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The service collection being modified.</param>
		/// <param name="reader">A <see cref="ConfigReader"/> that can read the JSON config file specified in <paramref name="configPath"/>.</param>
		/// <param name="configPath">The location of the JSON config to run.</param>
		/// <param name="debug">Indicates whether to attach a debugger.</param>
		/// <param name="silent">Indicates whether to output results to the console.</param>
		static void AddRunAction(this IServiceCollection services, ConfigReader reader, FileInfo configPath, FileInfo resultsPath,bool debug, bool silent)
		{
			if (debug)
				services.AddSingleton(new Debug() { AttachDebugger = true });

			var config = reader.FromFile(configPath.FullName);
			config.__filePath = configPath.FullName;

			if (!string.IsNullOrWhiteSpace(configPath.Directory.FullName))
				Directory.SetCurrentDirectory(configPath.Directory.FullName);

			if (resultsPath != null)
			{
				config.Execution.ResultsFilePath = resultsPath.FullName;
				config.Execution.ResultsToFile = true;
			}

			if (silent)
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
