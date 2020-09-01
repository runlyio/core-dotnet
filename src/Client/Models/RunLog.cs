using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Client.Models
{
	public enum RunLogType { stdout, stderr }

	public class RunLog
	{
		public long Id { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public RunLogType Type { get; set; }

		public int Index { get; set; }

		public string Value { get; set; }
	}

	public class RunLogInfo
	{
		public int Size { get; set; }
		public string DownloadUrl { get; set; }
	}
}
