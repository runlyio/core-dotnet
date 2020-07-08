using System;
using Newtonsoft.Json;

namespace Runly
{
	public class Error
	{
		public string Message { get; }
		public string Trace { get; }

		[JsonConstructor]
		public Error(string message, string trace)
		{
			Message = message;
			Trace = trace;
		}

		public Error(Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException(nameof(ex));

			Trace = ex.ToDetailedString();

			while (ex.InnerException != null)
				ex = ex.InnerException;

			Message = ex.Message;
		}

		public override string ToString() => Message;
	}
}
