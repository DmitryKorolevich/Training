using System;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Services;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.ExportService
{
    public class ExportDependencyConfig : DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            base.StartCustomServicesRegistration(services);
            services.AddSingleton<IServer, DummyServer>();
            services.AddDbContext<ExportInfoContext>().AddDbContext<ExportInfoCopyContext>();
            services.AddScoped<IOrderExportService, OrderExportService>();
            services.AddScoped<IVeraCoreExportService, VeraCoreExportService>();
            //services.AddScoped<EncryptionKeyUpdater>();
            services.AddSingleton<EncryptedServiceBusHostServer>();
            services.AddSingleton<IGiftListCreditCardExportFileGenerator, GiftListCreditCardExportFileGenerator>();
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
        {
            builder.RegisterType<DummyHostClient>().As<IEncryptedServiceBusHostClient>().SingleInstance();
        }

        protected override void ConfigureOptions(IConfiguration configuration, IServiceCollection services, IHostingEnvironment environment)
        {
            base.ConfigureOptions(configuration, services, environment);
            services.Configure<ExportOptions>(export =>
            {
                ConfigureAppOptions(configuration, export);
                var section = configuration.GetSection("App:ExportConnection");
                export.ExportConnection = new ExportDbConnection
                {
                    UserName = section.GetSection("UserName").Value,
                    Server = section.GetSection("Server").Value,
                    Password = section.GetSection("Password").Value,
                    AdminPassword = section.GetSection("AdminPassword").Value,
                    AdminUserName = section.GetSection("AdminUserName").Value,
                    Encrypt = Convert.ToBoolean(section.GetSection("Encrypt").Value)
                };
                var veraCoreSection = configuration.GetSection("App:VeraCore");
                export.VeraCore = new VeraCoreAuth
                {
                    UserName = veraCoreSection.GetSection("UserName").Value,
                    Password = veraCoreSection.GetSection("Password").Value
                };
                export.ScheduleDayTimeHour = int.Parse(configuration.GetSection("App:ScheduleDayTimeHour").Value);
            });
        }
    }
}