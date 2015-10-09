using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using VC.Public.AppConfig;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Infrastructure;

namespace VC.Public
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var applicationEnvironment = services.BuildServiceProvider().GetRequiredService<IApplicationEnvironment>();

            var configuration = new ConfigurationBuilder(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
            if (File.Exists(path))
            {
                configuration.AddJsonFile("config.local.json");
            }
            Configuration = configuration.Build();

            var reg = new DefaultDependencyConfig();
            return reg.RegisterInfrastructure(Configuration, services, null, typeof(Startup).GetTypeInfo().Assembly);
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseErrorPage(new ErrorPageOptions()
                {
                    SourceCodeLineCount = 25
                });
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

			//app.UseCookieAuthentication();

			app.UseMvc(RouteConfig.RegisterRoutes);
        }
    }
}