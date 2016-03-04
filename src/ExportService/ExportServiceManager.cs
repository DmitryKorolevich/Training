using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ExportServiceWithSBQueue.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus;
using Configuration = ExportServiceWithSBQueue.Configuration;

namespace ExportService
{
    public partial class ExportServiceManager : ServiceBase
    {
        private EncryptedServiceBusHostServer _server;
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly IContainer _container;

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
                _container = Configuration.BuildContainer();
                _options = _container.Resolve<IOptions<AppOptions>>();
                _logger = _container.Resolve<ILogger>();
                _encryptionHost = _container.Resolve<IObjectEncryptionHost>();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                throw;
            }
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Trace.WriteLine("Starting processing of messages");
            _server = new EncryptedServiceBusHostServer(_options, _logger, _container, _encryptionHost);
        }

        protected override void OnStop()
        {
            _server.Dispose();
            _container.Dispose();
            base.OnStop();
        }
    }
}