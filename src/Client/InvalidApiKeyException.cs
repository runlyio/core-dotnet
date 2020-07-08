using System;

namespace Runly
{
	public class InvalidApiKeyException : Exception
	{
		public InvalidApiKeyException()
			: base("Invalid API Key.") { }

		public InvalidApiKeyException(string message)
			: base(message) { }

		public InvalidApiKeyException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
