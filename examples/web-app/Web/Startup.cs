using Examples.WebApp.Web.Config;
using Examples.WebApp.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Runly;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Examples.WebApp.Web
{
	public class Startup
	{
		readonly IConfiguration cfg;
		readonly IWebHostEnvironment env;

		public Startup(IConfiguration cfg, IWebHostEnvironment env)
		{
			this.cfg = cfg;
			this.env = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			// Register the config options classes.
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options

			services.Configure<AppOptions>(o =>
			{
				// configure Sqlite to connect to database file relative to content root
				string dbPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "example.db"));
				o.ConnectionString = $"URI=file:{dbPath}";
			});

			// configure RunlyOptions from runly section of appsettings.json
			services.Configure<RunlyOptions>(cfg.GetSection("runly"));

			// Register our database connection. You could use an ORM here, but for the purposes
			// of this example we are going to use a raw SQLiteConnection with Dapper.
			services.AddScoped<DbConnection>(s => new SQLiteConnection(
				s.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value.ConnectionString
			));

			services.AddScoped<Database>();

			// Configure Runly API services to be available.
			services.AddRunlyApi(s =>
			{
				// pull the API key from our configuration file via the RunlyOptions class that we registered above
				var opts = s.GetRequiredService<IOptions<RunlyOptions>>().Value;
				return opts.SecretKey;
			});

			// Add our abstraction that will interface with Runly.
			services.AddTransient<IJobQueue, RunlyJobQueue>();
		}

		public void Configure(IApplicationBuilder app)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Invite}/{action=Index}/{id?}");
			});
		}
	}
}
