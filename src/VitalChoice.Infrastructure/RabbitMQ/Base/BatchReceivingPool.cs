using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public class BatchReceivingPool : RoundRobinAbstractPool<IEnumerable<BrokeredMessage>>
    {
        private readonly Action<IEnumerable<BrokeredMessage>> _processMessages;

        public BatchReceivingPool(byte maxThreads, ILogger logger, Action<IEnumerable<BrokeredMessage>> processMessages)
            : base(maxThreads, logger)
        {
            if (processMessages == null) throw new ArgumentNullException(nameof(processMessages));
            _processMessages = processMessages;
        }

        protected override void ProcessingAction(IEnumerable<BrokeredMessage> data, object localData, object processParameter)
            => _processMessages(data);
    }
}
