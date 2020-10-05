using Runly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.GettingStarted.FileSystem
{
	/// <summary>
	/// Copies a directory tree from its source path to the destination path.
	/// </summary>
	public class CopyDirectory : Job<CopyDirectoryConfig, string>
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

		public override IAsyncEnumerable<string> GetItemsAsync()
		{
			// The GetFilesIn method will return an IEnumerable<T> that will yield file names. GetItemsAsync wraps
			// the IEnumerable<T> in an AsyncEnumerableWrapper<T> so it can be accessed via the IAsyncEnumerable<T> 
			// interface. Because of this, the returned collection will be counted with the Linq extension Count() 
			// to determine the total number of items so that progress as a percentage can be displayed to users.
			// If the underlying data source was a stream in which there would be a high cost to enumerating the 
			// collection to obtain a count, the count can be disabled by setting the parameter canBeCounted to false.
			// See https://www.runly.io/docs/jobs/#total-count for more on this topic.
			return GetFilesIn(Config.Source).Select(file => Path.GetRelativePath(Config.Source, file)).ToAsyncEnumerable();
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
			catch (UnauthorizedAccessException) when (Config.IgnoreUnauthorizedAccessException)
			{
				return Result.Success("Skipped - Unauthorized", "Skipping file copy due to an UnauthorizedAccessException being thrown. Set IgnoreWhenAccessDenied = false to treat as an error.");
			}

			return Result.SuccessOrCancelled(CancellationToken);
		}
	}

	public class CopyDirectoryConfig : Config
	{
		public string Source { get; set; }
		public string Destination { get; set; }

		/// <summary>
		/// Causes an <see cref="UnauthorizedAccessException"/> to be ignored by skipping the file and returning a successful result.
		/// </summary>
		public bool IgnoreUnauthorizedAccessException { get; set; }
	}
}
