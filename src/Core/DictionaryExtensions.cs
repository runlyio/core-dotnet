using System.Collections.Generic;

namespace Runly
{
	/// <summary>
	/// Extension methods for <see cref="Dictionary{TKey, TValue}"/>.
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Gets a value stored in a <paramref name="dictionary"/> or the default value if the key is not present in the dictionary.
		/// </summary>
		/// <remarks>Returns the default value if <paramref name="dictionary"/> is null.</remarks>
		public static TValue ValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary == null)
				return default;

			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : default;
		}
	}
}
