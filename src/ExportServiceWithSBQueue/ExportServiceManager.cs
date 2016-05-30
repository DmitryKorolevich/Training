using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.ExportService.Services;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.ExportService
{
    public class ExportServiceManager : ServiceBase
    {
        private EncryptedServiceBusHostServer _server;
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly IServiceProvider _container;

        public ExportServiceManager()
        {
            try
            {
                Trace.Listeners.Add(new EventLogTraceListener(new EventLog("Application")
                {
                    Source = "ExportService"
                }));
                // Set the maximum number of concurrent connections 
                ServicePointManager.DefaultConnectionLimit = 12;
                _container = Program.Host.Services;
                _options = _container.GetRequiredService<IOptions<AppOptions>>();
                var factory = _container.GetRequiredService<ILoggerFactory>();
                _logger = factory.CreateLogger<ExportServiceManager>();
                _encryptionHost = _container.GetRequiredService<IObjectEncryptionHost>();
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
            Trace.WriteLine("Starting processing of messages");
            _server = new EncryptedServiceBusHostServer(_options, _logger, _container.GetRequiredService<ILifetimeScope>(), _encryptionHost);
        }

        protected override void OnStop()
        {
            _server.Dispose();
            Program.Host.Dispose();
            base.OnStop();
        }
    }
}