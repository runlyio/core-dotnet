using System.Threading.Tasks;

namespace Runly.Examples.Census
{
	public interface IDatabase
	{
		Task SavePlace(Place place);
	}

	public class FakeDatabase : IDatabase
	{
		public Task SavePlace(Place place)
		{
			// a real database would save this data...
			return Task.Delay(100);
		}
	}
}
