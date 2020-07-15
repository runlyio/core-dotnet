using System;

namespace Runly
{
	/// <summary>
	/// Exception thrown when there is a problem reading or validating a <see cref="Config"/>.
	/// </summary>
	public class ConfigException : Exception
	{
		/// <summary>
		/// The config parameter that caused the exception.
		/// </summary>
		public string ParamName { get; }

		/// <summary>
		/// Initializes a new <see cref="ConfigException"/>.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="paramName">The config parameter that caused the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public ConfigException(string message, string paramName, Exception innerException)
			: base(message, innerException)
		{
			this.ParamName = paramName;
		}

		/// <summary>
		/// Initializes a new <see cref="ConfigException"/>.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="paramName">The config parameter that caused the exception.</param>
		public ConfigException(string message, string paramName)
			: base(message)
		{
			this.ParamName = paramName;
		}
	}
}
