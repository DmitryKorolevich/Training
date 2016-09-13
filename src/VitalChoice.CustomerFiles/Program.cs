using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.CustomerFiles.Context;
using VitalChoice.CustomerFiles.Entities;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Interfaces.Services.Customers;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.CustomerFiles
{
    public class Program
    {
        public static IWebHost Host;

        public static void Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");
            Host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            Host.Start();

            try
            {
                var lifetimeScope = Program.Host.Services.GetRequiredService<ILifetimeScope>();
                using (var scope = lifetimeScope.BeginLifetimeScope())
                {
                    Console.WriteLine($"[{DateTime.Now:O}] Customer Files Import Start");
                    using (var context = scope.Resolve<OldDbContext>())
                    {
                        var set = context.Set<VCustomerOldFile>();
                        var allList = set.ToList();
                        var scopes = new ThreadLocal<ILifetimeScope>(() => lifetimeScope.BeginLifetimeScope());
                        var customerRepository =
                            new ThreadLocal<IEcommerceRepositoryAsync<Customer>>(() =>
                            {
                                var threadScope = scopes.Value;
                                return threadScope.Resolve<IEcommerceRepositoryAsync<Customer>>();
                            });
                        var customerService =
                            new ThreadLocal<ICustomerService>(() =>
                            {
                                var threadScope = scopes.Value;
                                return threadScope.Resolve<ICustomerService>();
                            });
                        Parallel.ForEach(allList.GroupBy(r => r.IdCustomer), new ParallelOptions
                        {
                            CancellationToken = CancellationToken.None,
                            MaxDegreeOfParallelism = 16,
                            TaskScheduler = TaskScheduler.Default
                        }, fileRecordGroup =>
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine($"Importing {fileRecordGroup.Key}");
                            Console.ResetColor();
                            List<Tuple<string, VCustomerOldFile>> uploadedFiles = new List<Tuple<string, VCustomerOldFile>>();
                            var customer =
                                customerRepository.Value.Query(f => f.Id == fileRecordGroup.Key)
                                    .Include(c => c.Files)
                                    .SelectFirstOrDefault(true);
                            if (customer != null)
                            {
                                Guid publicId = customer.PublicId;
                                if (customer.Files.Count != fileRecordGroup.Count())
                                {
                                    foreach (var fileRecord in fileRecordGroup)
                                    {
                                        if (File.Exists(fileRecord.FilePath))
                                        {
                                            Console.WriteLine($"Importing {fileRecord.OriginalFileName}");
                                            var content = File.ReadAllBytes(fileRecord.FilePath);
                                            uploadedFiles.Add(
                                                new Tuple<string, VCustomerOldFile>(
                                                    customerService.Value.UploadFileAsync(content, fileRecord.OriginalFileName, publicId)
                                                        .GetAwaiter()
                                                        .GetResult(), fileRecord));
                                        }
                                    }
                                    if (uploadedFiles.Any())
                                    {
                                        try
                                        {
                                            customer.Files.AddRange(uploadedFiles.Select(fileName => new CustomerFile
                                            {
                                                Description = fileName.Item2.Description,
                                                FileName = fileName.Item1,
                                                IdCustomer = customer.Id,
                                                UploadDate = fileName.Item2.UploadDate
                                            }));
                                            customerRepository.Value.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.ForegroundColor = ConsoleColor.DarkRed;
                                            Console.WriteLine($"[{e.Source}] Import Failed!\r\n{e}");
                                            Console.ResetColor();
                                            lock (lifetimeScope)
                                            {
                                                File.AppendAllText("errors.txt", $"{fileRecordGroup.Key}\r\n");
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"[{DateTime.Now:O}] Import Success!");
                Console.ResetColor();
                Program.Host.Dispose();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[{e.Source}] Import Failed!\r\n{e}");
                Console.ResetColor();
                Program.Host.Dispose();
            }
        }
    }
}
