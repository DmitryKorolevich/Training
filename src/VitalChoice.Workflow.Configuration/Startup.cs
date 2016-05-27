using System;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Configuration
{
    public class DummyServer : IServer
    {
        public void Dispose()
        {
        }

        public void Start<TContext>(IHttpApplication<TContext> application)
        {
            try
            {
                var lifetimeScope = Program.Host.Services.GetRequiredService<ILifetimeScope>();
                Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");
                using (var scope = lifetimeScope.BeginLifetimeScope())
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
            Program.Host.Dispose();
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();
    }

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
                .AddJsonFile("config.local.json", true);

            Configuration = builder.Build();
        }

        public void Configure(IApplicationBuilder app)
        {
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var reg = new WorkflowDependencyConfig();

            var container = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly, _hostingEnvironment, false);
            return container.Resolve<IServiceProvider>();
        }
    }
}