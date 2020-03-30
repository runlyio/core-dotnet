using Runly;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Examples.WebApp.Processes
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await ProcessHost.CreateDefaultBuilder(args)
				.ConfigureServices((host, services) =>
				{
					// Register the deps our process needs using the process config.
					// https://www.runly.io/docs/dependency-injection/#registering-dependencies

					services.AddScoped<DbConnection, InvitationEmailerConfig>((s, cfg) =>
						new SQLiteConnection(cfg.ConnectionString)
					);

					services.AddScoped<IEmailService, InvitationEmailerConfig>((s, cfg) =>
						new FakeEmailService(cfg.EmailServiceApiKey)
					);
				})
				.Build()
				.RunProcessAsync();
		}
	}
}
