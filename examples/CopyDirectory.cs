using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Runly.Examples
{
	public class CopyDirectory : Process<CopyDirectoryConfig, string>
	{
		public CopyDirectory(CopyDirectoryConfig config)
			: base(config) { }

		public override Task InitializeAsync()
		{
			if (!Directory.Exists(Config.Source))
				throw new ConfigException($"Directory not found: {Config.Source}", nameof(Config.Source));

			if (!Directory.Exists(Config.Destination))
				Directory.CreateDirectory(Config.Destination);

			return base.InitializeAsync();
		}

		public override Task<IEnumerable<string>> GetItemsAsync()
		{
			return Task.FromResult(GetFilesIn(Config.Source).Select(file => Path.GetRelativePath(Config.Source, file)));
		}

		private IEnumerable<string> GetFilesIn(string dir)
		{
			foreach (var file in Directory.GetFiles(dir))
				yield return file;

			foreach (var subdir in Directory.GetDirectories(dir))
				foreach (var file in GetFilesIn(subdir))
					yield return file;
		}

		public override async Task<Result> ProcessAsync(string file)
		{
			var sourceFile = Path.Combine(Config.Source, file);
			var destFile = Path.Combine(Config.Destination, file);

			if (File.Exists(destFile))
				return Result.Success("Already Copied");

			try
			{
				using (var source = File.Open(sourceFile, FileMode.Open))
				{
					var destDir = Path.GetDirectoryName(destFile);

					if (!Directory.Exists(destDir))
						Directory.CreateDirectory(destDir);

					using (var dest = File.Create(destFile))
					{
						await source.CopyToAsync(dest, CancellationToken);
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				if (Config.IgnoreUnauthorizedAccessException)
					return Result.Success("Skipped - Unauthorized", "Skipping file copy due to an UnauthorizedAccessException being thrown. Set IgnoreWhenAccessDenied = false to treat as an error.");
				else
					throw;
			}

			return Result.SuccessOrCancelled(CancellationToken);
		}
	}
}
