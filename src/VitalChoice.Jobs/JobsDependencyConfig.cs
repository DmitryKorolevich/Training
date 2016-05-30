using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Jobs.Infrastructure;

namespace VitalChoice.Jobs
{
    public class JobsDependencyConfig : DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            base.StartCustomServicesRegistration(services);
            services.AddSingleton<IServer, DummyServer>();
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
        {
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(JobsDependencyConfig).Assembly));
        }

        protected override void ConfigureAppOptions(IConfiguration configuration, AppOptions options)
        {
            base.ConfigureAppOptions(configuration, options);

            var items = configuration.GetSection("App:JobSettings:Schedules").GetChildren();

            options.JobSettings = new JobSettings()
            {
                Schedules = items.ToDictionary(x => x.Key, y => y.Value)
            };
        }
    }
}