using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runly
{
	public static class QueryString
	{
		public static string AddQueryString(this string baseUrl, string key, string value)
		{
			if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(value))
				return baseUrl;

			return AddQueryString(baseUrl, new[] { (key, value) });
		}

		public static string AddQueryString(this string baseUrl, IEnumerable<(string, string)> fields)
		{
			if (fields == null || !fields.Any())
				return baseUrl;

			StringBuilder url = new StringBuilder($"{baseUrl}?");

			foreach (var (key, value) in fields)
				url.Append($"{key}={Uri.EscapeDataString(value)}&");

			// remove the last ampersand
			url.Remove(url.Length - 1, 1);

			return url.ToString();
		}
	}
}