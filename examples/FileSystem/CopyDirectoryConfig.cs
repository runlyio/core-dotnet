using System;

namespace Runly.Examples.FileSystem
{
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
