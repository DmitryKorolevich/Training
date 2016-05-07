using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Tests
{
    public class TestDependencyConfig : DefaultDependencyConfig
    {
        protected override void FinishCustomRegistrations(ContainerBuilder builder)
        {
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