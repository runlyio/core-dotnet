using System;

namespace Runly
{
	public static class JsonStringExtensions
	{
		public static string ToCamelCase(this string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return value;

			value = value.Trim();
			return Char.ToLowerInvariant(value[0]) + value.Substring(1);
		}
	}
}
