using System.Diagnostics;
using System.Net;
using System.Threading;
using Autofac;
using ExportWorkerRoleWithSBQueue;
using ExportWorkerRoleWithSBQueue.Services;
using Microsoft.Azure;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.ServiceRuntime;
using VitalChoice.Infrastructure.Domain.Options;

namespace ExportWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");
            using (_container.Resolve<IEncryptedServiceBusHostServer>())
            {
                _completedEvent.WaitOne();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;
            _container = Configuration.BuildContainer();
            var options = _container.Resolve<IOptions<AppOptions>>();
            // Create the queue if it does not exist already
            var namespaceManager = NamespaceManager.CreateFromConnectionString(options.Value.ExportService.ConnectionString);
            var plainQueName = options.Value.ExportService.PlainQueueName;
            var encryptedQueName = options.Value.ExportService.EncryptedQueueName;
            if (!namespaceManager.QueueExists(plainQueName))
            {
                namespaceManager.CreateQueue(plainQueName);
            }
            if (!namespaceManager.QueueExists(encryptedQueName))
            {
                namespaceManager.CreateQueue(encryptedQueName);
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
