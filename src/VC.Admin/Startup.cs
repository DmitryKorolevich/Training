using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using VC.Admin.AppConfig;
using VitalChoice.Core.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.Business.Services;
using VitalChoice.Profiling;
using VitalChoice.Profiling.Base;

namespace VC.Admin
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var applicationEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(applicationEnvironment.WebRootPath)
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true)
                .AddEnvironmentVariables();


            Configuration = configuration.Build();

            var reg = new AdminDependencyConfig();

            var container = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly);
            return container.Resolve<IServiceProvider>();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions
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
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context => context.Context.Response.Headers.Add("Cache-Control", "public, max-age=604800")
            });

            app.UseIdentity();

            app.InjectProfiler();

            app.UseMvc(RouteConfig.RegisterRoutes);
        }
    }
}