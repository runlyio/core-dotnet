using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Internal
{
	class ListAction : IHostAction
	{
		readonly bool verbose;
		readonly bool json;
		readonly JobCache cache;
		readonly JsonSchema schema;

		public ListAction(bool verbose, bool json, JobCache cache, JsonSchema schema)
		{
			this.verbose = verbose;
			this.json = json;
			this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
			this.schema = schema ?? throw new ArgumentNullException(nameof(schema));
		}

		public async Task RunAsync(CancellationToken cancel)
		{
			string clientVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

			if (json)
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

		void WriteJson(TextWriter writer, string clientVersion)
		{
			var jobs = GetJobJson();
			writer.WriteJson(new { clientVersion, jobs });
		}

		IEnumerable GetJobJson()
		{
			if (verbose)
			{
				return cache.Jobs.OrderBy(i => i.JobType.FullName).Select(p => new
				{
					JobType = p.JobType.FullName,
					ConfigType = p.ConfigType.FullName,
					DefaultConfig = cache.GetDefaultConfig(p.JobType.FullName),
					Assembly = p.JobType.Assembly.GetName().Name,
					CanRun = p.IsValid,
					Errors = p.Errors.ToString(),
					Schema = schema.Generate(p.ConfigType)
				});
			}

			// Include the reduced config
			return cache.Jobs.OrderBy(i => i.JobType.FullName).Select(p => new
			{
				JobType = p.JobType.FullName,
				ConfigType = p.ConfigType.FullName,
				DefaultConfig = ConfigWriter.ToReducedJObject(cache.GetDefaultConfig(p.JobType.FullName)),
				Assembly = p.JobType.Assembly.GetName().Name,
				CanRun = p.IsValid,
				Errors = p.Errors.ToString()
			});
		}

		void WritePlainText(TextWriter writer, string clientVersion)
		{
			writer.WriteLine($"Client Version: v{clientVersion}");
			writer.WriteLine();

			foreach (var job in cache.Jobs.OrderBy(i => i.JobType.FullName))
			{
				writer.WriteLine(ConsoleFormat.DoubleLine);
				writer.WriteLine($"Job:\t{job.JobType.FullName} [{job.JobType.Assembly.GetName().Name}]");
				writer.WriteLine($"Config:\t{job.ConfigType.FullName} [{job.ConfigType.Assembly.GetName().Name}]");
				writer.WriteLine(ConsoleFormat.DoubleLine);
				writer.WriteLine();

				if (job.IsValid)
				{
					writer.WriteLine(verbose ? ConfigWriter.ToJson(cache.GetDefaultConfig(job)) : ConfigWriter.ToReducedJson(cache.GetDefaultConfig(job)));
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
