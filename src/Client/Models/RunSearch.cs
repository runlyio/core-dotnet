using System;
using System.Collections.Generic;

namespace Runly.Client.Models
{
	public class RunSearch
	{
		public string Environment { get; set; }

		public string Package { get; set; }
		public string Job { get; set; }
		public string Version { get; set; }

		public Guid? StartingAfter { get; set; }
		public Guid? EndingBefore { get; set; }
		public int? Limit { get; set; }

		public string User { get; set; }

		public RunDisposition Disposition { get; set; }

		public IEnumerable<(string, string)> ToQuerystring()
		{
			var dic = new List<(string, string)>();

			if (!String.IsNullOrWhiteSpace(Environment))
				dic.Add(("environment", Environment));

			if (!String.IsNullOrWhiteSpace(Package))
				dic.Add(("package", Package));

			if (!String.IsNullOrWhiteSpace(Job))
				dic.Add(("job", Job));

			if (!String.IsNullOrWhiteSpace(Version))
				dic.Add(("version", Version));

			if (StartingAfter.HasValue)
				dic.Add(("startingAfter", StartingAfter.Value.ToString()));

			if (EndingBefore.HasValue)
				dic.Add(("endingBefore", EndingBefore.Value.ToString()));

			if (Limit.HasValue)
				dic.Add(("limit", Limit.Value.ToString()));

			if (!String.IsNullOrWhiteSpace(User))
				dic.Add(("user", User));

			if (Disposition != default(RunDisposition))
				dic.Add(("disposition", Disposition.ToString()));

			return dic;
		}
	}

	public enum RunDisposition
	{
		All,
		NotSuccessful,
		Successful,
		Failed,
		Cancelled,
		TimedOut,
		Error
	}
}
