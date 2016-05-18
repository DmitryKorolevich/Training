using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Quartz;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Jobs.Jobs;

namespace VitalChoice.Jobs
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var servicesToRun = new ServiceBase[]
            {
                new JobWindowsService(PlatformServices.Default.Application)
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}