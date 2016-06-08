using System;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.CustomerFiles.Context;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.CustomerFiles
{
    public class ImportDependencyConfig: DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            base.StartCustomServicesRegistration(services);
            services.AddSingleton<IServer, DummyServer>();
            services.AddDbContext<OldDbContext>();
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
        {
            base.FinishCustomRegistrations(builder);
            builder.RegisterType<OldDbContext>().AsSelf();
        }
    }
}