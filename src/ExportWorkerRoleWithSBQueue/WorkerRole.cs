using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ExportWorkerRoleWithSBQueue.Services;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace ExportWorkerRoleWithSBQueue
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "ProcessingQueue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        private QueueClient _client;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            _client.OnMessageAsync(receivedMessage =>
                {
                    using (var scope = _container.BeginLifetimeScope())
                    {
                        var exportService = scope.Resolve<IOrderExportService>();
                        switch (receivedMessage.ContentType)
                        {
                            case "application/order-export":
                                var exportData = receivedMessage.GetBody<OrderExportData>();
                                return Task.WhenAll(exportData.ExportInfo.Select(e => exportService.ExportOrder(e.Id, e.OrderType)));
                            case "application/update-order-payment":
                                break;
                            case "application/update-customer-payment":
                                break;
                        }
                        return Task.Delay(0);
                    }
                });

            _completedEvent.WaitOne();
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

            // Initialize the connection to Service Bus Queue
            _client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            _container = Configuration.BuildContainer();
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            _client.Close();
            _completedEvent.Set();
            _container.Dispose();
            base.OnStop();
        }
    }
}
