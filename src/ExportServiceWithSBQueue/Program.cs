using System.IO;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;

namespace VitalChoice.ExportService
{
    public static class Program
    {
        public static IWebHost Host { get; set; }

        public static void Main()
        {
            Host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            Host.Start();

            var servicesToRun = new ServiceBase[]
            {
                new ExportServiceInstance()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}