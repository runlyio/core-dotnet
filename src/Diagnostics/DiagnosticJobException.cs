using System;

namespace Runly.Diagnostics
{
	[Serializable]
	public class DiagnosticJobException : Exception
	{
		public JobMethod Location { get; private set; }

		public DiagnosticJobException(JobMethod location, string message)
			: base(message)
		{
			Location = location;
		}
	}
}
