using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Hosting
{
	internal class ListAction : HostedAction
	{
		private readonly bool _verbose;
		private readonly bool _json;
		private readonly JobCache _cache;
		private readonly JsonSchema _schema;
		private readonly IHostApplicationLifetime _applicationLifetime;

		public ListAction(bool verbose, bool json, JobCache cache, JsonSchema schema, IHostApplicationLifetime applicationLifetime)
		{
			_verbose = verbose;
			_json = json;
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_schema = schema ?? throw new ArgumentNullException(nameof(schema));
			_applicationLifetime = applicationLifetime;
		}

		protected override async Task RunAsync(CancellationToken cancel)
		{
			try
			{
				string clientVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

				if (_json)
				{
					WriteJson(Console.Out, clientVersion);
				}
				else
				{
					WritePlainText(Console.Out, clientVersion);
				}

				// Ensure the entire output can be read by the node
				Console.WriteLine();
				await Console.Out.FlushAsync();
            }
			finally
			{
				_applicationLifetime?.StopApplication();
			}
        }

		void WriteJson(TextWriter writer, string clientVersion)
		{
			var jobs = GetJobJson();
			writer.WriteJson(new { clientVersion, jobs });
		}

		IEnumerable GetJobJson()
		{
			if (_verbose)
			{
				return _cache.Jobs.OrderBy(i => i.JobType.FullName).Select(p => new
				{
					JobType = p.JobType.FullName,
					ConfigType = p.ConfigType.FullName,
					DefaultConfig = _cache.GetDefaultConfig(p.JobType.FullName),
					Assembly = p.JobType.Assembly.GetName().Name,
					CanRun = p.IsValid,
					Errors = p.Errors.ToString(),
					Schema = _schema.Generate(p.ConfigType)
				});
			}

			// Include the reduced config
			return _cache.Jobs.OrderBy(i => i.JobType.FullName).Select(p => new
			{
				JobType = p.JobType.FullName,
				ConfigType = p.ConfigType.FullName,
				DefaultConfig = ConfigWriter.ToReducedJObject(_cache.GetDefaultConfig(p.JobType.FullName)),
				Assembly = p.JobType.Assembly.GetName().Name,
				CanRun = p.IsValid,
				Errors = p.Errors.ToString()
			});
		}

		void WritePlainText(TextWriter writer, string clientVersion)
		{
			writer.WriteLine($"Client Version: v{clientVersion}");
			writer.WriteLine();

			foreach (var job in _cache.Jobs.OrderBy(i => i.JobType.FullName))
			{
				writer.WriteLine(ConsoleFormat.DoubleLine);
				writer.WriteLine($"Job:\t{job.JobType.FullName} [{job.JobType.Assembly.GetName().Name}]");
				writer.WriteLine($"Config:\t{job.ConfigType.FullName} [{job.ConfigType.Assembly.GetName().Name}]");
				writer.WriteLine(ConsoleFormat.DoubleLine);
				writer.WriteLine();

				if (job.IsValid)
				{
					writer.WriteLine(_verbose ? ConfigWriter.ToJson(_cache.GetDefaultConfig(job)) : ConfigWriter.ToReducedJson(_cache.GetDefaultConfig(job)));
					writer.WriteLine();
				}
				else
				{
					writer.WriteLine($"Error Loading Job: {job.Errors.ToString()}");
					writer.WriteLine();
				}
			}
		}
	}
}
