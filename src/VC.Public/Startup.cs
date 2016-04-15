using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VC.Public.AppConfig;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNet.Http;
using System.Globalization;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Cookies;
using VitalChoice.Interfaces.Services;
using Autofac;
using Microsoft.AspNet.StaticFiles;
using VitalChoice.Profiling;

namespace VC.Public
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
                .AddJsonFile("config.local.json", true)
                .AddEnvironmentVariables();

            Configuration = configuration.Build();

            var reg = new StorefrontDependencyConfig();
            var container = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly);

			return container.Resolve<IServiceProvider>();
		}

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage(new ErrorPageOptions()
                {
                    SourceCodeLineCount = 25
                });
                //app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseExceptionHandler("/Shared/Error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context => context.Context.Response.Headers.Add("Cache-Control", "public, max-age=3600")
            });

            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();

            app.UseSession();
            app.InjectProfiler();
            app.Use(async (context, next) =>
            {
                var redirectViewService = context.RequestServices.GetService<IRedirectViewService>();
                var result = redirectViewService.CheckRedirects(context);

                if (!result)
                {
                    await next();
                }
            });
            app.UseStatusCodePagesWithReExecute("/help/error/{0}");
            app.UseMvc(RouteConfig.RegisterRoutes);
        }
    }
}