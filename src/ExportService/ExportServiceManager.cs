using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ExportServiceWithSBQueue;
using ExportServiceWithSBQueue.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportService
{
    public partial class ExportServiceManager : ServiceBase
    {
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private readonly IOptions<AppOptions> _options;
        private readonly ILogger _logger;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly IContainer _container;

        public ExportServiceManager()
        {
            try
            {
                if (!EventLog.SourceExists("ExportService"))
                {
                    EventLog.CreateEventSource("ExportService", "Application");
                }
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

                //var namespaceManager =
                //    NamespaceManager.CreateFromConnectionString(_options.Value.ExportService.ConnectionString);
                //var plainQueName = _options.Value.ExportService.PlainQueueName;
                //var encryptedQueName = _options.Value.ExportService.EncryptedQueueName;
                //if (!namespaceManager.QueueExists(plainQueName))
                //{
                //    namespaceManager.CreateQueue(plainQueName);
                //}
                //if (!namespaceManager.QueueExists(encryptedQueName))
                //{
                //    namespaceManager.CreateQueue(encryptedQueName);
                //}
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Trace.WriteLine("Starting processing of messages");
            using (new EncryptedServiceBusHostServer(_options, _logger, _container, _encryptionHost))
            {
                _completedEvent.WaitOne();
            }
        }

        protected override void OnStop()
        {
            _completedEvent.Set();
            _container.Dispose();
            base.OnStop();
        }
    }
}
