using Dapper;
using Runly;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.WebApp.Jobs
{
	public class InvitationEmailer : Job<InvitationEmailerConfig, string, DbConnection, IEmailService>
	{
		int count = 0;
		readonly DbConnection db;

		public InvitationEmailer(InvitationEmailerConfig config, DbConnection db)
			: base(config)
		{
			this.db = db;
		}

		public override async Task InitializeAsync()
		{
			await db.OpenAsync();
		}

		public override IAsyncEnumerable<string> GetItemsAsync()
		{
			// run our database query to get the emails we want to send
			return db.QueryAsync<string>("select [Email] from [User] where HasBeenEmailed = 0").ToAsyncEnumerable();
		}

		public override async Task<Result> ProcessAsync(string email, DbConnection db, IEmailService emails)
		{
			// use method-scoped db rather than the class-scoped db for parallel tasks
			// https://www.runly.io/docs/dependency-injection/#method-injection

			// fake a failure for every 100th item
			int i = Interlocked.Increment(ref count);
			if (i % 100 == 0)
				return Result.Failure("Internet Down");

			// send our fake email
			await emails.SendEmail(email, "You are invited!", "Join us. We have cake.");

			// Open the connection if it is not already opened. Since we register the DbConnection as Scoped,
			// a previous call to ProcessAsync with this Task could have opened the connection. In that case,
			// the connection would already be open. Though multiple parallel tasks could be calling ProcessAsync
			// at the same time, the DbConnection is used only with a single Task.
			if (db.State != ConnectionState.Open)
				await db.OpenAsync();

			// mark the user as invited in the database
			await db.ExecuteAsync("update [User] set HasBeenEmailed = 1 where [Email] = @email", new { email });

			return Result.Success();
		}
	}

	public class InvitationEmailerConfig : Config
	{
		public string ConnectionString { get; set; }
		public string EmailServiceApiKey { get; set; }
	}
}
