using Dapper;
using System.Data.Common;
using System.Threading.Tasks;

namespace Examples.WebApp.Web.Services
{
	public class Database
	{
		readonly DbConnection db;

		public Database(DbConnection db)
		{
			this.db = db;
		}

		public async Task Init()
		{
			await db.OpenAsync();

			await db.ExecuteAsync("drop table if exists [User]");

			await db.ExecuteAsync(@"create table [User] (
				Id integer primary key autoincrement,
				Email varchar(250) not null,
				HasBeenEmailed bit not null default 0
			)");
		}
	}
}
