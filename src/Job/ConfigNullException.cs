namespace Runly
{
	public class ConfigNullException : ConfigException
	{
		public ConfigNullException(string paramName)
			: base($"Config parameter cannot be null: {paramName}", paramName) { }
	}
}
