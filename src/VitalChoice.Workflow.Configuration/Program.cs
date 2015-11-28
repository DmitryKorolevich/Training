using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Core;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Core.Infrastructure;
using Microsoft.Extensions.PlatformAbstractions;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Workflow.Configuration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");

                var container = BuildContainer();
                using (var scope = container.BeginLifetimeScope())
                {
                    Console.WriteLine($"[{DateTime.Now:O}] Configuring DB");
                    var setup = scope.Resolve<ITreeSetup<OrderDataContext, decimal>>();
                    DefaultConfiguration.Configure(setup);
                    if (setup.UpdateAsync().Result)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[{DateTime.Now:O}] Update Success!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[{DateTime.Now:O}] Update Failed! See logs for details.");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[{e.Source}] Update Failed!\r\n{e.Message}\r\n{e.StackTrace}");
                Console.ResetColor();
            }
        }

        private static IContainer BuildContainer()
        {
            var applicationEnvironment =
                (IApplicationEnvironment)
                    CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (IApplicationEnvironment));
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json");

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
            if (File.Exists(path))
            {
                configurationBuilder.AddJsonFile("config.local.json");
            }
            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddSqlServer();

            services.Configure<AppOptionsBase>(options =>
            {
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
            });

            services.Configure<AppOptions>(options =>
            {
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.DefaultCultureId = configuration.GetSection("App:DefaultCultureId").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterInstance(configuration).As<IConfiguration>();
            builder.RegisterInstance(
                LoggerService.Build(applicationEnvironment.ApplicationBasePath,
                    configuration.GetSection("App:LogPath").Value))
                .As<ILoggerProviderExtended>().SingleInstance();

            builder.Register((cc, pp) => cc.Resolve<ILoggerProviderExtended>().CreateLogger("Root")).As<ILogger>();
            builder.RegisterGeneric(typeof(Logger<>))
                .WithParameter((pi, cc) => pi.ParameterType == typeof(ILoggerFactory),
                    (pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
                .As(typeof(ILogger<>));

            var container = new AdminDependencyConfig().BuildContainer(typeof (Program).GetTypeInfo().Assembly, builder);
            return container;
        }
    }
}