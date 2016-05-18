﻿using System;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Workflow.Configuration
{
    public class Program
    {
        public void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");

                var container = BuildContainer();
                using (var scope = container.BeginLifetimeScope())
                {
                    Console.WriteLine($"[{DateTime.Now:O}] Configuring DB");
                    var setups = DefaultConfiguration.Configure(scope);
                    var setupCleaner = scope.Resolve<ITreeSetupCleaner>();
                    var transactionAccessor = scope.Resolve<ITransactionAccessor<EcommerceContext>>();
                    using (var transaction = transactionAccessor.BeginTransaction())
                    {
                        try
                        {
                            if (!setupCleaner.CleanAllTrees().GetAwaiter().GetResult())
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine($"[{DateTime.Now:O}] DB Clean Failed! See logs for details.");
                                Console.ResetColor();
                            }
                            else
                            {
                                foreach (var setup in setups)
                                {
                                    if (setup.CreateTreesAsync().GetAwaiter().GetResult())
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine($"[{DateTime.Now:O}] {setup.GetType().FullName} Update Success!");
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.WriteLine($"[{DateTime.Now:O}] {setup.GetType().FullName} Update Failed! See logs for details.");
                                        Console.ResetColor();
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"[{e.Source}] Update Failed!\r\n{e.Message}\r\n{e.StackTrace}");
                            Console.ResetColor();
                            transaction.Rollback();
                        }
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
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true);

            //var path = PathResolver.ResolveAppRelativePath("config.local.json");
            //if (File.Exists(path))
            //{
            //    configurationBuilder.AddJsonFile("config.local.json");
            //}
            var configuration = configurationBuilder.Build();
            var builder = new ContainerBuilder();
            var host = new WebHostBuilder().ConfigureServices(services =>
            {
                services.AddEntityFramework().AddEntityFrameworkSqlServer();

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
                builder.Populate(services);
                builder.RegisterInstance(configuration).As<IConfiguration>();
                builder.RegisterType<LoggerProviderExtended>()
                    .As<ILoggerProviderExtended>()
                    .As<ILoggerProvider>()
                    .SingleInstance();
                builder.Register(cc => cc.Resolve<ILoggerProviderExtended>().Factory).As<ILoggerFactory>();

                builder.Register((cc, pp) => cc.Resolve<ILoggerProviderExtended>().CreateLogger("Root")).As<ILogger>();
                builder.RegisterGeneric(typeof(Logger<>))
                    .WithParameter((pi, cc) => pi.ParameterType == typeof(ILoggerFactory),
                        (pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
                    .As(typeof(ILogger<>));
            }).Build();
            builder.RegisterInstance(host.Services.GetService<IHostingEnvironment>());
            var container = new AdminDependencyConfig().BuildContainer(typeof(Program).GetTypeInfo().Assembly, builder);
            LoggerService.Build(container.Resolve<IHostingEnvironment>());
            return container;
        }
    }
}