using Newtonsoft.Json.Converters;

namespace Runly.Client.Models
{
	public class DateWithoutTimeConverter : IsoDateTimeConverter
	{
		public DateWithoutTimeConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
