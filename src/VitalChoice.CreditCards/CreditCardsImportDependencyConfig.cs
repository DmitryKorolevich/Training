using System;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.DependencyInjection;

namespace VitalChoice.CreditCards
{
    public class CreditCardsImportDependencyConfig: DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            base.StartCustomServicesRegistration(services);
            services.AddSingleton<IServer, DummyServer>();
            services.AddDbContext<ExportInfoContext>();
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
        {
            base.FinishCustomRegistrations(builder);
            builder.RegisterType<ExportInfoContext>().AsSelf();
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