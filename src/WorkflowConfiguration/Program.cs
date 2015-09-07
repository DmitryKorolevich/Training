using System;
using Autofac;
using Microsoft.Dnx.Runtime;
using Microsoft.Dnx.Runtime.Infrastructure;
using Microsoft.Framework.Configuration;
using System.Reflection;
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

namespace Workflow.Configuration
{
    public class Program
    {

        public async void Main(string[] args)
        {
            var applicationEnvironment =
                (IApplicationEnvironment)
                    CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (IApplicationEnvironment));
            var configurationBuilder = new ConfigurationBuilder(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("config.json");
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
            using (var scope = container.BeginLifetimeScope())
            {
                var setup = scope.Resolve<ITreeSetup<OrderContext, decimal>>();
                DefaultConfiguration.Configure(setup);
                await setup.UpdateAsync();
            }
        }
    }
}