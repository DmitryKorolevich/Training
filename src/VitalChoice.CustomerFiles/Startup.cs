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