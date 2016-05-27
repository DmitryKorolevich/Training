using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Workflow.Core;
using VitalChoice.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Workflow.Configuration
{
    public class Program
    {
        public void Main(string[] args)
        {
            try
            {
                var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

                host.Run();

                Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");

                var container = host.Services.GetRequiredService<ILifetimeScope>();
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
    }
}