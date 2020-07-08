using System;

namespace Runly
{
	public class ConfigException : ApplicationException
	{
		public string ParamName { get; }

		public ConfigException(string message, string paramName, Exception innerException)
			: base(message, innerException)
		{
			this.ParamName = paramName;
		}

		public ConfigException(string message, string paramName)
			: base(message)
		{
			this.ParamName = paramName;
		}
	}
}
