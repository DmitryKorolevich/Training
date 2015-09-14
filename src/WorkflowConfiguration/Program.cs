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

namespace Workflow.Configuration
{
    public class Program
    {

        public async Task Main(string[] args)
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
                options.GenerateLowercaseUrls =
                    Convert.ToBoolean(configuration.GetSection("App:GenerateLowercaseUrls").Value);
                options.EnableBundlingAndMinification =
                    Convert.ToBoolean(configuration.GetSection("App:EnableBundlingAndMinification").Value);
                options.Versioning = new Versioning()
                {
                    EnableStaticContentVersioning =
                        Convert.ToBoolean(configuration.GetSection("App:Versioning:EnableStaticContentVersioning").Value),
                    BuildNumber =
                        Convert.ToBoolean(configuration.GetSection("App:Versioning:AutoGenerateBuildNumber").Value)
                            ? Guid.NewGuid().ToString("N")
                            : configuration.GetSection("App:Versioning:BuildNumber").Value
                };
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.DefaultCacheExpirationTermMinutes =
                    Convert.ToInt32(configuration.GetSection("App:DefaultCacheExpirationTermMinutes").Value);
                options.ActivationTokenExpirationTermDays =
                    Convert.ToInt32(configuration.GetSection("App:ActivationTokenExpirationTermDays").Value);
                options.DefaultCultureId = configuration.GetSection("App:DefaultCultureId").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
                options.PublicHost = configuration.GetSection("App:PublicHost").Value;
                options.AdminHost = configuration.GetSection("App:AdminHost").Value;
                options.FilesRelativePath = configuration.GetSection("App:FilesRelativePath").Value;
                options.EmailConfiguration = new Email
                {
                    From = configuration.GetSection("App:Email:From").Value,
                    Host = configuration.GetSection("App:Email:Host").Value,
                    Port = Convert.ToInt32(configuration.GetSection("App:Email:Port").Value),
                    Secured = Convert.ToBoolean(configuration.GetSection("App:Email:Secured").Value),
                    Username = configuration.GetSection("App:Email:Username").Value,
                    Password = configuration.GetSection("App:Email:Password").Value
                };
                options.AzureStorage = new AzureStorage()
                {
                    StorageConnectionString = configuration.GetSection("App:AzureStorage:StorageConnectionString").Value,
                    CustomerContainerName = configuration.GetSection("App:AzureStorage:CustomerContainerName").Value
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