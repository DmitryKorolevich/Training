using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace VitalChoice.Tests
{
    public static class TestsDependencyContainer
    {
        public static ILifetimeScope RootScope { get; }

        static TestsDependencyContainer()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true);

            var configuration = configurationBuilder.Build();

            RootScope = new TestDependencyConfig().RegisterInfrastructure(configuration, new ServiceCollection(),
                typeof(TestDependencyConfig).Assembly, PlatformServices.Default.Application, enableCache: true);
        }
    }
}