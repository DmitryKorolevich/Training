using System;
using System.IO;
using Autofac;
using Microsoft.AspNetCore.Hosting;
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
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();

            RootScope = host.Services.GetRequiredService<ILifetimeScope>();
        }
    }
}