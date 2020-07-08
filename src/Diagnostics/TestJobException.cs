using System;

namespace Runly.Diagnostics
{
	[Serializable]
	public class TestJobException : Exception
	{
		public JobMethod Location { get; private set; }

		public TestJobException(JobMethod location, string message)
			: base(message)
		{
			Location = location;
		}
	}
}
