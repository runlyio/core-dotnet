using System;

namespace Runly
{
	public class AuthenticationException : Exception
	{
		public AuthenticationException()
			: base("Authentication failed.") { }

		public AuthenticationException(Exception innerException)
			: base("Authentication failed.", innerException) { }
	}
}
