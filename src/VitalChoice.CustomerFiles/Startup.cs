using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.CustomerFiles.Context;
using VitalChoice.CustomerFiles.Entities;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.CustomerFiles
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
                using (var scope = lifetimeScope.BeginLifetimeScope())
                {
                    Console.WriteLine($"[{DateTime.Now:O}] Customer Files Import Start");
                    using (var context = scope.Resolve<OldDbContext>())
                    {
                        var set = context.Set<VCustomerOldFile>();
                        var allList = set.ToList();
                        ThreadLocal<ILifetimeScope> scopes = new ThreadLocal<ILifetimeScope>(() => lifetimeScope.BeginLifetimeScope());
                        ThreadLocal<IEcommerceRepositoryAsync<Customer>> customerRepository =
                            new ThreadLocal<IEcommerceRepositoryAsync<Customer>>(() =>
                            {
                                var threadScope = scopes.Value;
                                return threadScope.Resolve<IEcommerceRepositoryAsync<Customer>>();
                            });
                        ThreadLocal<ICustomerService> customerService =
                            new ThreadLocal<ICustomerService>(() =>
                            {
                                var threadScope = scopes.Value;
                                return threadScope.Resolve<ICustomerService>();
                            });
                        List<Task> taskList = new List<Task>();
                        foreach (var group in allList.GroupBy(r => r.IdCustomer))
                        {
                            taskList.Add(
                                Task.Factory.StartNew(recordGroup =>
                                {
                                    var fileRecordGroup = (IGrouping<int, VCustomerOldFile>) recordGroup;
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine($"Importing {fileRecordGroup.Key}");
                                    Console.ResetColor();
                                    List<Tuple<string, VCustomerOldFile>> uploadedFiles = new List<Tuple<string, VCustomerOldFile>>();
                                    var customer =
                                        customerRepository.Value.Query(f => f.Id == fileRecordGroup.Key)
                                            .Include(c => c.Files)
                                            .SelectFirstOrDefault(true);
                                    Guid publicId = customer.PublicId;
                                    if (customer.Files.Count != fileRecordGroup.Count())
                                    {
                                        foreach (var fileRecord in fileRecordGroup)
                                        {
                                            if (File.Exists(fileRecord.Name))
                                            {
                                                Console.WriteLine($"Importing {fileRecord.OriginalFileName}");
                                                var content = File.ReadAllBytes(fileRecord.Name);
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
                                }, group));
                        }
                        Task.WhenAll(taskList).GetAwaiter().GetResult();
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
            }
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
            var reg = new ImportDependencyConfig();

            var container = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly, _hostingEnvironment, true);
            return container.Resolve<IServiceProvider>();
        }
    }
}