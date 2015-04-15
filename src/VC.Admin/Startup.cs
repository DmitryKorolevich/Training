using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using VitalChoice.Core.DependencyInjection;
using Microsoft.AspNet.Mvc;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Business.Services.Impl;
using System.IO;
using System.Net;
using Microsoft.Framework.Runtime;
using VitalChoice.Admin.AppConfig;

namespace VitalChoice
{
	public class Startup
	{
        public IConfiguration Configuration { get; set; }


		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            var applicationEnvironment = services.BuildServiceProvider().GetRequiredService<IApplicationEnvironment>();

            var configuration = new Configuration(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
            if (File.Exists(path)) {
                configuration.AddJsonFile("config.local.json");
            }
            Configuration = configuration;

            var reg = new DefaultDependencyConfig();

			return reg.RegisterInfrastructure(Configuration, services);
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
		{
			// Configure the HTTP request pipeline.
			// Add the console logger.
			loggerfactory.AddConsole();
            LoggerService.Build(env.WebRootPath, Configuration.Get("App:LogPath"));

            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
			{
				app.UseErrorPage(ErrorPageOptions.ShowAll);
				app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// send the request to the following path or controller action.
				app.UseErrorHandler("/Home/Error");
			}

			// Add static files to the request pipeline.
			app.UseStaticFiles();

			// Add cookie-based authentication to the request pipeline.
			app.UseIdentity();

            app.UseMvc(routes =>
            {
                RouteConfig.RegisterRoutes(routes);
            });
		}
	}
}
