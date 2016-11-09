using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class SendingPool : RoundRobinAbstractPool<BrokeredMessage>
    {
        private readonly IServiceBusSender _sender;

        public SendingPool(IServiceBusSender sender, ILogger logger) : base(1, logger)
        {
            _sender = sender;
        }

        protected override void ProcessingAction(BrokeredMessage data, object localData, object processParameter)
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