using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.Profiling;

namespace VC.Admin
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true)
                .AddJsonFile("hosting.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var reg = new AdminDependencyConfig();

            var container = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly, _hostingEnvironment);
            return container.Resolve<IServiceProvider>();
        }

        public void ConfigureDevelopment(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Information);

            app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions
            {
                SourceCodeLineCount = 25
            });

            Configure(app);
        }

        public void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            app.UseExceptionHandler("/Home/Error");

            Configure(app);
        }

        public void ConfigureProduction(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            app.UseExceptionHandler("/Home/Error");

            Configure(app);
        }

        public void Configure(IApplicationBuilder app)
        {
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