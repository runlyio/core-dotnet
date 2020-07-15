namespace Runly
{
	/// <summary>
	/// Exception thrown when there is a null config parameter that cannot be null.
	/// </summary>
	public class ConfigNullException : ConfigException
	{
		/// <summary>
		/// Initializes a new <see cref="ConfigNullException"/>.
		/// </summary>
		/// <param name="paramName">The config parameter that caused the exception.</param>
		public ConfigNullException(string paramName)
			: base($"Config parameter cannot be null: {paramName}", paramName) { }
	}
}
