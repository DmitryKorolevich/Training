using System;
using System.Globalization;
using System.IO;
using Autofac;
using Microsoft.Dnx.Runtime;
using Microsoft.Dnx.Runtime.Infrastructure;
using Microsoft.Framework.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.Framework.DependencyInjection;
using VitalChoice.Core.Base;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Infrastructure.Context;
using Autofac.Framework.DependencyInjection;
using Microsoft.AspNet.Identity.EntityFramework;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Core.Infrastructure;

namespace VitalChoice.Workflow.Configuration
{
    public class Program
    {

        public async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");

                var container = BuildContainer();
                using (var scope = container.BeginLifetimeScope())
                {
                    Console.WriteLine($"[{DateTime.Now:O}] Configuring DB");
                    var setup = scope.Resolve<ITreeSetup<OrderContext, decimal>>();
                    DefaultConfiguration.Configure(setup);
                    if (await setup.UpdateAsync())
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
            var configurationBuilder = new ConfigurationBuilder(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json");

            var path = PathResolver.ResolveAppRelativePath("config.local.json");
            if (File.Exists(path))
            {
                configurationBuilder.AddJsonFile("config.local.json");
            }
            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<VitalChoiceContext>();

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

            var container = DefaultDependencyConfig.BuildContainer(typeof (Program).GetTypeInfo().Assembly, builder);
            return container;
        }
    }
}