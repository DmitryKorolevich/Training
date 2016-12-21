using System;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public class ReceivingPool : RoundRobinAbstractPool<BrokeredMessage>
    {
        private readonly Action<BrokeredMessage> _processMessage;

        public ReceivingPool(byte maxThreads, ILogger logger, Action<BrokeredMessage> processMessage) : base(maxThreads, logger)
        {
            if (processMessage == null) throw new ArgumentNullException(nameof(processMessage));
            _processMessage = processMessage;
        }

        protected override void ProcessingAction(BrokeredMessage data, object localData, object processParameter) => _processMessage(data);
    }
}