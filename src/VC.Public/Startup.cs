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

namespace VitalChoice.Public
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Setup configuration sources.
			Configuration = new Configuration()
				.AddJsonFile("config.json")
				.AddEnvironmentVariables();
		}

		public IConfiguration Configuration { get; set; }


		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            var reg = new DefaultDependencyConfig();
            return reg.RegisterInfrastructure(Configuration, services);
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
		{
            // Configure the HTTP request pipeline.
            // Add the console logger.
            loggerfactory.AddConsole();
            LoggerService.Build(env.WebRootPath);

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
