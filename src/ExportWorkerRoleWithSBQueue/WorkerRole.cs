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
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
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

            _client.OnMessage(receivedMessage =>
            {
                switch (receivedMessage.ContentType)
                {
                    case OrderExportServiceConstants.ExportOrder:
                        var exportData = receivedMessage.GetBody<OrderExportData>();
                        //List<OrderExportError> errors = new List<OrderExportError>();
                        Parallel.ForEach(exportData.ExportInfo, e =>
                        {
                            using (var scope = _container.BeginLifetimeScope())
                            {
                                var exportService = scope.Resolve<IOrderExportService>();
                                ICollection<string> errors = null;
                                bool success;
                                try
                                {
                                    success = exportService.ExportOrder(e.Id, e.OrderType, out errors);
                                }
                                catch (Exception ex)
                                {
                                    if (errors == null)
                                    {
                                        errors = new List<string>();
                                    }
                                    errors.Add(ex.Message);
                                    success = false;
                                }
                                string contentType = receivedMessage.ContentType;
                                string sessionId = receivedMessage.SessionId;
                                receivedMessage.Complete();
                                
                                _client.Send(new BrokeredMessage(new OrderExportItemResult
                                {
                                    Id = e.Id,
                                    Success = success,
                                    Errors = errors
                                })
                                {
                                    ContentType = contentType,
                                    SessionId = sessionId
                                });
                            }
                        });
                        break;
                    case OrderExportServiceConstants.UpdateOrderPayment:
                        var orderPaymentInfo = receivedMessage.GetBody<OrderPaymentMethodDynamic>();
                        using (var scope = _container.BeginLifetimeScope())
                        {
                            var exportService = scope.Resolve<IOrderExportService>();
                            exportService.UpdatePaymentMethod(orderPaymentInfo);
                            receivedMessage.Complete();
                        }
                        break;
                    case OrderExportServiceConstants.UpdateCustomerPayment:
                        var customerPaymentInfo = receivedMessage.GetBody<CustomerPaymentMethodDynamic>();
                        using (var scope = _container.BeginLifetimeScope())
                        {
                            var exportService = scope.Resolve<IOrderExportService>();
                            exportService.UpdatePaymentMethod(customerPaymentInfo);
                            receivedMessage.Complete();
                        }
                        break;
                    default:
                        using (var scope = _container.BeginLifetimeScope())
                        {
                            var logger = scope.Resolve<ILogger>();
                            logger.LogWarning($"Cannot process message with ContentType {receivedMessage.ContentType}");
                            receivedMessage.Complete();
                        }
                        break;
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
