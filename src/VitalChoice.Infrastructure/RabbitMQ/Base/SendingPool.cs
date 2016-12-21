using Microsoft.Extensions.Logging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public class SendingPool<T> : RoundRobinAbstractPool<T>
    {
        private readonly IServiceBusSender<T> _sender;

        public SendingPool(IServiceBusSender<T> sender, ILogger logger) : base(1, logger)
        {
            _sender = sender;
        }

        protected override void ProcessingAction(T data, object localData, object processParameter) => _sender.Send(data);
    }
}