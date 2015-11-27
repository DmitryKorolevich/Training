using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportWorkerRoleWithSBQueue
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "ProcessingQueue";
        const string RespondQueueName = "RespondQueue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        private QueueClient _client;
        private QueueClient _respondClient;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private IContainer _container;
        private readonly ObjectEncryptionHost _objectEncryptionHost = new ObjectEncryptionHost();
        

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            _client.OnMessage(ProcessMessage);

            _completedEvent.WaitOne();
        }

        private void ProcessMessage(BrokeredMessage receivedMessage)
        {
            switch (receivedMessage.ContentType)
            {
                case OrderExportServiceConstants.GetPublicKey:
                    _client.Send(new BrokeredMessage(_objectEncryptionHost.PublicKey)
                    {
                        SessionId = receivedMessage.SessionId,
                        ContentType = receivedMessage.ContentType,
                        MessageId = receivedMessage.MessageId
                    });
                    receivedMessage.Complete();
                    break;
                case OrderExportServiceConstants.SetSessionKey:
                    var sessionKey = Guid.Parse(receivedMessage.SessionId);
                    var key = _objectEncryptionHost.RsaDecrypt<KeyExchange>(receivedMessage.GetBody<byte[]>());
                    if (_objectEncryptionHost.RegisterSession(sessionKey, key))
                    {
                        _client.Send(new BrokeredMessage(true)
                        {
                            SessionId = receivedMessage.SessionId,
                            ContentType = receivedMessage.ContentType,
                            MessageId = receivedMessage.MessageId
                        });
                    }
                    receivedMessage.Complete();
                    break;
                case OrderExportServiceConstants.CheckSessionKey:
                    sessionKey = Guid.Parse(receivedMessage.SessionId);
                    _client.Send(new BrokeredMessage(_objectEncryptionHost.SessionExist(sessionKey))
                    {
                        SessionId = receivedMessage.SessionId,
                        ContentType = receivedMessage.ContentType,
                        MessageId = receivedMessage.MessageId
                    });
                    
                    receivedMessage.Complete();
                    break;
                case OrderExportServiceConstants.ExportOrder:
                    var exportData = _objectEncryptionHost.AesDecrypt<OrderExportData>(receivedMessage.GetBody<byte[]>(),
                        Guid.Parse(receivedMessage.SessionId));
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

                            _respondClient.Send(new BrokeredMessage(new OrderExportItemResult
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
                    var orderPaymentInfo =
                        _objectEncryptionHost.AesDecrypt<OrderPaymentMethodDynamic>(receivedMessage.GetBody<byte[]>(),
                            Guid.Parse(receivedMessage.SessionId));
                    using (var scope = _container.BeginLifetimeScope())
                    {
                        var exportService = scope.Resolve<IOrderExportService>();
                        exportService.UpdatePaymentMethod(orderPaymentInfo);
                        receivedMessage.Complete();
                    }
                    break;
                case OrderExportServiceConstants.UpdateCustomerPayment:
                    var customerPaymentInfo =
                        _objectEncryptionHost.AesDecrypt<CustomerPaymentMethodDynamic>(receivedMessage.GetBody<byte[]>(),
                            Guid.Parse(receivedMessage.SessionId));
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

            // Initialize the connection to Service Bus Queue
            _client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            _respondClient = QueueClient.CreateFromConnectionString(connectionString, RespondQueueName);
            _container = Configuration.BuildContainer();
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            _client.Close();
            _respondClient.Close();
            _completedEvent.Set();
            _container.Dispose();
            base.OnStop();
        }
    }
}
