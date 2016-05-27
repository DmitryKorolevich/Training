using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Workflow.Core;
using VitalChoice.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.Data.Transaction;

namespace VitalChoice.Workflow.Configuration
{
    public class Program
    {
        public static IWebHost Host;

        public static void Main(string[] args)
        {
            Host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            Host.Run();
        }
    }
}