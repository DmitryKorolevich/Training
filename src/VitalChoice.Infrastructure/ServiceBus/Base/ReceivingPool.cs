using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ReceivingPool : RoundRobinAbstractPool<BrokeredMessage>
    {
        private readonly Action<BrokeredMessage> _processMessage;

        public ReceivingPool(byte maxThreads, ILogger logger, Action<BrokeredMessage> processMessage) : base(maxThreads, logger)
        {
            if (processMessage == null) throw new ArgumentNullException(nameof(processMessage));
            _processMessage = processMessage;
        }

        protected override void ProcessingAction(BrokeredMessage data)
        {
            _processMessage(data);
        }
    }
}