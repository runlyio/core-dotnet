using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Hosting
{
	internal class GetAction : HostedAction
	{
		private readonly bool _verbose;
		private string _filePath;
		private readonly string _jobType;
		private readonly JobCache _cache;
		private readonly IHostApplicationLifetime _applciationLifetime;

		internal GetAction(bool verbose, string jobType, string filePath, JobCache cache, IHostApplicationLifetime applicationLifetime)
		{
			_verbose = verbose;
			_jobType = jobType;
			_filePath = filePath;
			_cache = cache;
			_applciationLifetime = applicationLifetime;
		}

        protected override async Task RunAsync(CancellationToken cancel)
		{
			try
			{
				TextWriter writer = null;
				JobInfo job;

				try
				{
					job = _cache.Get(_jobType);
				}
				catch (TypeNotFoundException)
				{
					Console.WriteLine($"Could not find the job type '{_jobType}'.");
					return;
				}

				var config = _cache.GetDefaultConfig(job);

				try
				{

					if (_filePath == null)
					{
						writer = Console.Out;
					}
					else
					{
						// If path is an existing directory, such as ".", add a file name
						if (Directory.Exists(_filePath))
							_filePath = Path.Combine(_filePath, job.JobType.Name + ".json");

						writer = new StreamWriter(File.Open(_filePath, FileMode.Create));
					}

					await writer.WriteAsync(_verbose ? ConfigWriter.ToJson(config) : ConfigWriter.ToReducedJson(config));
				}
				finally
				{
					if (_filePath != null && writer != null)
					{
						await writer.FlushAsync();
						writer.Dispose();
					}
				}

				if (_filePath != null)
					Console.WriteLine($"Default config for {job.JobType.FullName} saved to {Path.GetFullPath(_filePath)}");
			}
			finally
			{
				_applciationLifetime?.StopApplication();
			}
		}
	}
}
