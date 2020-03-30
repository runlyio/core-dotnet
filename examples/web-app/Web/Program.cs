using Examples.WebApp.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Examples.WebApp.Web
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// create the host that will run our web application
			var host = CreateHostBuilder(args).Build();

			// initialize the database before we start the host
			await InitDatabase(host.Services);

			// start the web host
			await host.RunAsync();
		}

		static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		static async Task InitDatabase(IServiceProvider services)
		{
			using (var scope = services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<Database>();
				await db.Init();
			}
		}
	}
}
