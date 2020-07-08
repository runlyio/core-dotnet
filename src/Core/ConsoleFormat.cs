using System.Text;

namespace Runly
{
	public static class ConsoleFormat
	{
		public static string DoubleLine => new string('=', 60);

		public static string SingleLine => new string('-', 60);

		public static string AsPlural(int count, string ending = "s") => count > 1 ? ending : string.Empty;

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
