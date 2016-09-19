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
        //private EncryptionKeyUpdater _keyUpdater;
        private ILogger _logger;
        private ILifetimeScope _startupScope;

        public ExportServiceInstance()
        {
            try
            {
                // Set the maximum number of concurrent connections 
                ServicePointManager.DefaultConnectionLimit = 1000;
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

                var rootScope = Host.Services.GetRequiredService<ILifetimeScope>();
                var factory = rootScope.Resolve<ILoggerFactory>();
                _logger = factory.CreateLogger<ExportServiceInstance>();
                Trace.WriteLine("Starting processing of messages");
                _server = rootScope.Resolve<EncryptedServiceBusHostServer>();
                _startupScope = rootScope.BeginLifetimeScope();
                //_keyUpdater = _startupScope.Resolve<EncryptionKeyUpdater>();
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
            //_keyUpdater.Dispose();
            _startupScope.Dispose();
            Host.Dispose();
            base.OnStop();
        }
    }
}