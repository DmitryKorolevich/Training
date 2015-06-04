using System;
using System.IO;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;
using Newtonsoft.Json;
using VC.Public.AppConfig;
using VitalChoice.Business.Services;
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

            var configuration = new Configuration( /*applicationEnvironment.ApplicationBasePath*/)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
            if (File.Exists(path))
            {
                configuration.AddJsonFile("config.local.json");
            }
            Configuration = configuration;

            services.Configure<MvcOptions>(o =>
            {
                var inputFormatter =
                    (JsonInputFormatter)
                        o.InputFormatters.SingleOrDefault(f => f.Instance is JsonInputFormatter)?.Instance;
                var outputFormatter =
                    (JsonOutputFormatter)
                        o.OutputFormatters.SingleOrDefault(f => f.Instance is JsonOutputFormatter)?.Instance;

                if (inputFormatter != null)
                {
                    inputFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    inputFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                    inputFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                }
                else
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var newFormatter = new JsonInputFormatter();
                    newFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    newFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                    newFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    o.InputFormatters.Add(newFormatter);
                }

                if (outputFormatter != null)
                {
                    outputFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    outputFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                    outputFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                }
                else
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var newFormatter = new JsonOutputFormatter();
                    newFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    newFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                    newFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    o.OutputFormatters.Add(newFormatter);
                }
            });

            var reg = new DefaultDependencyConfig();
            return reg.RegisterInfrastructure(Configuration, services, null);
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