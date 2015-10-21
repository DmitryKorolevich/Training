using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using VC.Admin.AppConfig;
using VitalChoice.Business.Services;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.DynamicData.Services;


namespace VC.Admin
{
	public class Startup
	{
        public IConfiguration Configuration { get; set; }
		
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            var applicationEnvironment = services.BuildServiceProvider().GetRequiredService<IApplicationEnvironment>();
		    var configuration = new ConfigurationBuilder()
		        .SetBasePath(applicationEnvironment.ApplicationBasePath)
		        .AddJsonFile("config.json")
		        .AddEnvironmentVariables();

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
			if (File.Exists(path))
			{
				configuration.AddJsonFile("config.local.json");
			}
			Configuration = configuration.Build();

            var reg = new AdminDependencyConfig();

			var filesPath = Configuration.GetSection("App:FilesPath").Value;
            var result = reg.RegisterInfrastructure(Configuration, services, filesPath, typeof(Startup).GetTypeInfo().Assembly);
            return result;
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
		{
            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage(new ErrorPageOptions
                {
                    SourceCodeLineCount = 150
                });
				//app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// send the request to the following path or controller action.
			}
			// Add static files to the request pipeline.
			app.UseStaticFiles();

			app.UseIdentity();

			app.UseCookieAuthentication();

			app.UseMvc(RouteConfig.RegisterRoutes);
		}
	}
}