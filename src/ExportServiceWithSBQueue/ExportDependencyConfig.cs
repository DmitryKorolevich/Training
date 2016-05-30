using System;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.ExportService
{
    public class ExportDependencyConfig : DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            base.StartCustomServicesRegistration(services);
            services.AddSingleton<IServer, DummyServer>();
        }

        protected override void ConfigureAppOptions(IConfiguration configuration, AppOptions options)
        {
            base.ConfigureAppOptions(configuration, options);
            var section = configuration.GetSection("App:ExportService");
            options.ExportService = new Infrastructure.Domain.Options.ExportService
            {
                PlainConnectionString = section.GetSection("PlainConnectionString").Value,
                EncryptedConnectionString = section.GetSection("EncryptedConnectionString").Value,
                EncryptedQueueName = section.GetSection("EncryptedQueueName").Value,
                PlainQueueName = section.GetSection("PlainQueueName").Value,
                CertThumbprint = section.GetSection("CertThumbprint").Value,
                RootThumbprint = section.GetSection("RootThumbprint").Value,
                EncryptionHostSessionExpire = Convert.ToBoolean(section.GetSection("EncryptionHostSessionExpire").Value),
                ServerHostName = section.GetSection("ServerHostName").Value
            };
        }
    }
}