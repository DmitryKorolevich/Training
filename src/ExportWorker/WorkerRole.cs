using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Autofac;
using ExportWorkerRoleWithSBQueue;
using ExportWorkerRoleWithSBQueue.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.ServiceRuntime;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private IOptions<AppOptions> _options;
        private ILogger _logger;
        private IObjectEncryptionHost _encryptionHost;
        private IContainer _container;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");
            using (new EncryptedServiceBusHostServer(_options, _logger, _container, _encryptionHost))
            {
                _completedEvent.WaitOne();
            }
        }

        public override bool OnStart()
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

                var namespaceManager =
                    NamespaceManager.CreateFromConnectionString(_options.Value.ExportService.ConnectionString);
                var plainQueName = _options.Value.ExportService.PlainQueueName;
                var encryptedQueName = _options.Value.ExportService.EncryptedQueueName;
                if (!namespaceManager.QueueExists(plainQueName))
                {
                    namespaceManager.CreateQueue(plainQueName);
                }
                if (!namespaceManager.QueueExists(encryptedQueName))
                {
                    namespaceManager.CreateQueue(encryptedQueName);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
            return base.OnStart();
        }

        public override void OnStop()
        {
            _completedEvent.Set();
            _container.Dispose();
            base.OnStop();
        }
    }
}
