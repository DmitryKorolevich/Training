using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class SendingPool : RoundRobinAbstractPool<BrokeredMessage>
    {
        public SendingPool(byte maxThreads, ) : base(maxThreads)
        {
        }

        protected override void ProcessingAction(BrokeredMessage data)
        {
            throw new NotImplementedException();
        }
    }
}
