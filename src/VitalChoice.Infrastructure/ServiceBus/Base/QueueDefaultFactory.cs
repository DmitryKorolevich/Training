using System;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class QueueDefaultFactory : IServiceBusClientFactory<QueueClient>
    {
        protected readonly string ConnectionString;
        protected readonly string QueName;
        protected readonly ReceiveMode ReceiveMode;

        public QueueDefaultFactory(string connectionString, string queName, ReceiveMode receiveMode)
        {
            ConnectionString = connectionString;
            QueName = queName;
            ReceiveMode = receiveMode;
        }

        public QueueClient Create()
        {
            var plainFactory = MessagingFactory.CreateFromConnectionString(ConnectionString);
            return plainFactory.CreateQueueClient(QueName, ReceiveMode);
        }
    }

    public class TopicDefaultFactory : IServiceBusClientFactory<TopicClient>
    {
        protected readonly string ConnectionString;
        protected readonly string QueName;

        public TopicDefaultFactory(string connectionString, string queName)
        {
            ConnectionString = connectionString;
            QueName = queName;
        }

        public TopicClient Create()
        {
            EnsureTopicExists();
            var plainFactory = MessagingFactory.CreateFromConnectionString(ConnectionString);
            return plainFactory.CreateTopicClient(QueName);
        }

        protected virtual void EnsureTopicExists()
        {
            var ns = NamespaceManager.CreateFromConnectionString(ConnectionString);
            if (!ns.TopicExists(QueName))
            {
                TopicDescription topic = new TopicDescription(QueName)
                {
                    EnableExpress = true,
                    EnablePartitioning = true,
                    EnableBatchedOperations = true,
                    DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                    RequiresDuplicateDetection = false
                };

                ns.CreateTopic(topic);
            }
        }
    }

    public class SubcriptionDefaultFactory : IServiceBusClientFactory<SubscriptionClient>
    {
        protected readonly string ConnectionString;
        protected readonly string QueName;
        protected readonly string AppHostName;
        protected readonly ReceiveMode ReceiveMode;

        public SubcriptionDefaultFactory(string connectionString, string queName, string appHostName, ReceiveMode receiveMode)
        {
            ConnectionString = connectionString;
            QueName = queName;
            AppHostName = appHostName;
            ReceiveMode = receiveMode;
        }

        public SubscriptionClient Create()
        {
            EnsureSubscriptionExists();
            var plainFactory = MessagingFactory.CreateFromConnectionString(ConnectionString);
            return plainFactory.CreateSubscriptionClient(QueName, AppHostName, ReceiveMode);
        }

        protected virtual void EnsureSubscriptionExists()
        {
            var ns = NamespaceManager.CreateFromConnectionString(ConnectionString);
            if (!ns.SubscriptionExists(QueName, AppHostName))
            {
                SubscriptionDescription subscription = new SubscriptionDescription(QueName, AppHostName)
                {
                    EnableBatchedOperations = true,
                    DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                    RequiresSession = false,
                    EnableDeadLetteringOnFilterEvaluationExceptions = false,
                    EnableDeadLetteringOnMessageExpiration = false,
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(20),
                    MaxDeliveryCount = 1
                };
                ns.CreateSubscription(subscription);
            }
        }
    }
}