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
using VitalChoice.Core.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNet.Http;
using System.Globalization;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;
using VitalChoice.Interfaces.Services;

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
                .AddEnvironmentVariables();

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
            if (File.Exists(path))
            {
                configuration.AddJsonFile("config.local.json");
            }
            Configuration = configuration.Build();

            var reg = new StorefrontDependencyConfig();
            var result = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly);
            return result;
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
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();

			app.UseCookieAuthentication();

            app.UseSession();
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
            app.UseCookieAuthentication(x =>
            {
                x.AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType;
                x.LogoutPath = "/Account/Logout";
                x.AccessDeniedPath = "/Shared/AccessDenied";
                x.LoginPath = "/Account/Login";
                x.ReturnUrlParameter = "returnUrl";
                x.CookieName = "VitalChoice.Public";
            });
        }
    }
}