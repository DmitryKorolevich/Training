using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace VitalChoice.Jobs
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
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true);

            Configuration = builder.Build();
        }

        public void Configure(IApplicationBuilder app)
        {
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var reg = new JobsDependencyConfig();

            var container = reg.RegisterInfrastructure(Configuration, services, typeof(Startup).GetTypeInfo().Assembly, _hostingEnvironment, true);
            return container.Resolve<IServiceProvider>();
        }
    }
}