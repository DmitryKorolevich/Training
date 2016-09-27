using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class SendingPool : RoundRobinAbstractPool<BrokeredMessage>
    {
        private readonly IServiceBusSender _sender;

        public SendingPool(byte maxThreads, IServiceBusSender sender, ILogger logger) : base(maxThreads, logger)
        {
            _sender = sender;
        }

        protected override void ProcessingAction(BrokeredMessage data)
        {
            if (data.Size < 196608)
            {
                _sender.Send(data);
            }
            else
            {
                Logger.LogWarning($"Message too big: {data.Size} bytes, {data.ContentType}");
            }
        }
    }
}