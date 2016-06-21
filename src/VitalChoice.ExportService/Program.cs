using System.IO;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;

namespace VitalChoice.ExportService
{
    public static class Program
    {
        public static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new ExportServiceInstance()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}