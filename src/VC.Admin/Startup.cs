using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using VC.Admin.AppConfig;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Infrastructure;
using Microsoft.AspNet.Identity;

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
                .AddJsonFile("config.local.json", true)
                .AddEnvironmentVariables();

            
			Configuration = configuration.Build();

            var reg = new AdminDependencyConfig();

            var result = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly);
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
                    SourceCodeLineCount = 250
                });
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

		    app.UseCookieAuthentication(x =>
		    {
		        x.AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType;
		        x.LogoutPath = PathString.Empty;
		        x.AccessDeniedPath = PathString.Empty;
		        x.LoginPath = PathString.Empty;
		        x.ReturnUrlParameter = null;
		        x.CookieName = "VitalChoice.Admin";
		    });
            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.IsHttps)
            //    {
            //        await next();
            //    }
            //    else
            //    {
            //        var withHttps = "https://" + context.Request.Host + context.Request.Path;
            //        context.Response.Redirect(withHttps);
            //    }
            //});
        }
	}
}