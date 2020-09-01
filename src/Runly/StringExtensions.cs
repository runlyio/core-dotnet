using System;

namespace Runly
{
	/// <summary>
	/// Extension methods for strings.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Trims the string and converts the first character to lower case.
		/// </summary>
		public static string ToCamelCase(this string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return value;

			value = value.Trim();
			return Char.ToLowerInvariant(value[0]) + value.Substring(1);
		}
	}
}
