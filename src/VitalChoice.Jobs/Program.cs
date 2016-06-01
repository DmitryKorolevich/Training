using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;

namespace VitalChoice.Jobs
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var servicesToRun = new ServiceBase[]
            {
                new JobWindowsService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}