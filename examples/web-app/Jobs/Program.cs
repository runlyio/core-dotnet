using Runly;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Examples.WebApp.Jobs
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await JobHost.CreateDefaultBuilder(args)
				.ConfigureServices((host, services) =>
				{
					// Register the deps our job needs using the job config.
					// https://www.runly.io/docs/dependency-injection/#registering-dependencies

					services.AddScoped<DbConnection, InvitationEmailerConfig>((s, cfg) =>
						new SQLiteConnection(cfg.ConnectionString)
					);

					services.AddScoped<IEmailService, InvitationEmailerConfig>((s, cfg) =>
						new FakeEmailService(cfg.EmailServiceApiKey)
					);
				})
				.Build()
				.RunJobAsync();
		}
	}
}
