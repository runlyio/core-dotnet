using System;

namespace Runly
{
	public class RunlyApiNotFoundException : Exception
	{
		public RunlyApiNotFoundException(string wrongUrl)
			: base($"Could not connect to the Runly API at the url {wrongUrl}") { }
	}
}
