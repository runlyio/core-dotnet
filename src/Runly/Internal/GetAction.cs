using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Runly.Internal
{
	class GetAction : IHostAction
	{
		readonly bool verbose;
		string filePath;
		readonly string jobType;
		readonly JobCache cache;

		internal GetAction(bool verbose, string jobType, string filePath, JobCache cache)
		{
			this.verbose = verbose;
			this.jobType = jobType;
			this.filePath = filePath;
			this.cache = cache;
		}

		public async Task RunAsync(CancellationToken cancel)
		{
			TextWriter writer = null;
			JobInfo job;

			try
			{
				job = cache.Get(jobType);
			}
			catch (TypeNotFoundException)
			{
				Console.WriteLine($"Could not find the job type '{jobType}'.");
				return;
			}

			var config = cache.GetDefaultConfig(job);

			try
			{

				if (filePath == null)
				{
					writer = Console.Out;
				}
				else
				{
					// If path is an existing directory, such as ".", add a file name
					if (Directory.Exists(filePath))
						filePath = Path.Combine(filePath, job.JobType.Name + ".json");

					writer = new StreamWriter(File.Open(filePath, FileMode.Create));
				}
					
				await writer.WriteAsync(verbose ? ConfigWriter.ToJson(config) : ConfigWriter.ToReducedJson(config));
			}
			finally
			{
				if (filePath != null && writer != null)
				{
					await writer.FlushAsync();
					writer.Dispose();
				}
			}
		
			if (filePath != null)
				Console.WriteLine($"Default config for {job.JobType.FullName} saved to {Path.GetFullPath(filePath)}");
		}
	}
}
