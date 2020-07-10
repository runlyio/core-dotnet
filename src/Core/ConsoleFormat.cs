using System.Text;

namespace Runly
{
	/// <summary>
	/// Methods for formatting text written to the <see cref="System.Console"/>.
	/// </summary>
	public static class ConsoleFormat
	{
		/// <summary>
		/// A double line made up of sixty '=' characters.
		/// </summary>
		public static string DoubleLine => new string('=', 60);

		/// <summary>
		/// A line made up of sixty '-' characters.
		/// </summary>
		public static string SingleLine => new string('-', 60);

		/// <summary>
		/// Returns the line ending to pluralize a word when the count is greater than one.
		/// </summary>
		public static string AsPlural(int count, string ending = "s") => count > 1 ? ending : string.Empty;

		/// <summary>
		/// Creates a single line string from the <paramref name="data"/>, appending space or truncating text to fit it into columns.
		/// </summary>
		/// <param name="width">The width, in characters, of a single column.</param>
		/// <param name="data">The data to go in each column.</param>
		/// <returns>A string with the data spaced out into columns.</returns>
		public static string AsColumns(int width, params string[] data)
		{
			var row = new StringBuilder();
			int i = 0;

			foreach (string col in data)
			{
				i++;
				if (col == null)
				{
					row.Append(new string(' ', width));
				}
				else if (col.Length >= width)
				{
					if (i < data.Length)
						row.Append(col.Substring(0, width));
					else
						row.Append(col);
				}
				else
				{
					row.Append(col);
					row.Append(new string(' ', width - col.Length));
				}
			}

			return row.ToString();
		}
	}
}
