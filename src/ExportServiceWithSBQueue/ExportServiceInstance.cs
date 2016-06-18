using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.ExportService.Services;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.ExportService
{
    public class ExportServiceInstance : ServiceBase
    {
        private EncryptedServiceBusHostServer _server;
        private ILogger _logger;
        private IServiceProvider _container;

        public ExportServiceInstance()
        {
            try
            {
                Trace.Listeners.Add(new EventLogTraceListener(new EventLog("Application")
                {
                    Source = "ExportService"
                }));
                // Set the maximum number of concurrent connections 
                ServicePointManager.DefaultConnectionLimit = 12;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
            ServiceName = "exportservice";
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            RequestAdditionalTime(30000);

            try
            {

                Host = new WebHostBuilder()
                    .UseContentRoot(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]))
                    .UseStartup<Startup>()
                    .Build();

                Host.Start();

                _container = Host.Services;
                var factory = _container.GetRequiredService<ILoggerFactory>();
                _logger = factory.CreateLogger<ExportServiceInstance>();
                Trace.WriteLine("Starting processing of messages");
                _server = _container.GetRequiredService<EncryptedServiceBusHostServer>();
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    $"Env Command Line: {string.Join(", ", Environment.GetCommandLineArgs())}\n Command Line: {string.Join(", ", args)} {e}");
            }
        }

        public IWebHost Host { get; set; }

        protected override void OnStop()
        {
            _server.Dispose();
            Host.Dispose();
            base.OnStop();
        }
    }
}