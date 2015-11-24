using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace ExportWorkerRoleWithSBQueue
{
    internal class Configuration
    {
        internal static IContainer BuildContainer()
        {
            var applicationEnvironment =
                (IApplicationEnvironment)
                    CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IApplicationEnvironment));
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
                .AddSqlServer()
                .AddDbContext<VitalChoiceContext>();

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
            builder.RegisterGeneric(typeof (Logger<>))
                .WithParameter((pi, cc) => pi.ParameterType == typeof (ILoggerFactory),
                    (pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
                .As(typeof (ILogger<>));

            var container = new AdminDependencyConfig().BuildContainer(typeof(WorkerRole).GetTypeInfo().Assembly, builder);
            return container;
        }
    }
}