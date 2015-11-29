using System.Diagnostics;
using System.Net;
using System.Threading;
using Autofac;
using ExportWorkerRoleWithSBQueue.Services;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ExportWorkerRoleWithSBQueue
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "ProcessingQueue";
        const string RespondQueueName = "RespondQueue";

        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");
            var hostServer = _container.Resolve<IEncryptedServiceBusHostServer>();
            _completedEvent.WaitOne();
            hostServer.Dispose();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }
            if (!namespaceManager.QueueExists(RespondQueueName))
            {
                namespaceManager.CreateQueue(RespondQueueName);
            }
            _container = Configuration.BuildContainer();
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
