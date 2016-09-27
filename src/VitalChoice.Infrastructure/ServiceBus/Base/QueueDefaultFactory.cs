using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class QueueDefaultFactory : IServiceBusClientFactory<QueueClient>
    {
        private readonly string _connectionString;
        private readonly string _queName;
        private readonly ReceiveMode _receiveMode;

        public QueueDefaultFactory(string connectionString, string queName, ReceiveMode receiveMode)
        {
            _connectionString = connectionString;
            _queName = queName;
            _receiveMode = receiveMode;
        }

        public QueueClient Create()
        {
            var plainFactory = MessagingFactory.CreateFromConnectionString(_connectionString);
            return plainFactory.CreateQueueClient(_queName, _receiveMode);
        }
    }

    public class TopicDefaultFactory : IServiceBusClientFactory<TopicClient>
    {
        private readonly string _connectionString;
        private readonly string _queName;

        public TopicDefaultFactory(string connectionString, string queName)
        {
            _connectionString = connectionString;
            _queName = queName;
        }

        public TopicClient Create()
        {
            var plainFactory = MessagingFactory.CreateFromConnectionString(_connectionString);
            return plainFactory.CreateTopicClient(_queName);
        }
    }

    public class SubcriptionDefaultFactory : IServiceBusClientFactory<SubscriptionClient>
    {
        private readonly string _connectionString;
        private readonly string _queName;
        private readonly string _appHostName;
        private readonly ReceiveMode _receiveMode;

        public SubcriptionDefaultFactory(string connectionString, string queName, string appHostName, ReceiveMode receiveMode)
        {
            _connectionString = connectionString;
            _queName = queName;
            _appHostName = appHostName;
            _receiveMode = receiveMode;
        }

        public SubscriptionClient Create()
        {
            var plainFactory = MessagingFactory.CreateFromConnectionString(_connectionString);
            return plainFactory.CreateSubscriptionClient(_queName, _appHostName, _receiveMode);
        }
    }
}