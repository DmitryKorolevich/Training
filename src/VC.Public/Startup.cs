using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.Interfaces.Services;
using Autofac;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Primitives;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Core.Services;

namespace VC.Public
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }

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
            var reg = new StorefrontDependencyConfig();
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

            app.UseSession();
            app.Use((context, next) =>
            {
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Cache-Control"] = "private, max-age=0, no-cache, no-store";
                context.Response.Headers["Expires"] = "-1";
                var redirectViewService = context.RequestServices.GetService<IRedirectViewService>();
                var result = redirectViewService.CheckRedirects(context);

                if (!result)
                {
                    return next();
                }
                return TaskCache.CompletedTask;
            });
            app.UseStatusCodeExecutePath("/content/" + ContentConstants.NOT_FOUND_PAGE_URL, HttpStatusCode.NotFound);
            app.UseStatusCodeExecutePath("/content/" + ContentConstants.ACESS_DENIED_PAGE_URL, HttpStatusCode.Forbidden);
            app.UseMvc(RouteConfig.RegisterRoutes);
        }
    }
}