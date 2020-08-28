using Newtonsoft.Json.Converters;

namespace Runly.Models
{
	public class DateWithoutTimeConverter : IsoDateTimeConverter
	{
		public DateWithoutTimeConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}
}
