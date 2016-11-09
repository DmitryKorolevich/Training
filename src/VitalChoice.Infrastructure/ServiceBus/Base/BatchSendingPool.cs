using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class BatchSendingPool : RoundRobinAbstractPool<IEnumerable<BrokeredMessage>>
    {
        private readonly IServiceBusSender _sender;

        public BatchSendingPool(IServiceBusSender sender, ILogger logger) : base(1, logger)
        {
            _sender = sender;
        }

        protected override void ProcessingAction(IEnumerable<BrokeredMessage> data, object localData, object processParameter)
        {
            var extraList = new Lazy<List<BrokeredMessage>>(() => new List<BrokeredMessage>());
            _sender.SendBatch(GetCompleteBatch(data, extraList));
            if (extraList.IsValueCreated)
            {
                EnqueueData(extraList.Value);
            }
        }

        private IEnumerable<BrokeredMessage> GetCompleteBatch(IEnumerable<BrokeredMessage> messages, Lazy<List<BrokeredMessage>> extraList)
        {
            long batchSize = 0;
            foreach (var message in messages)
            {
                if (message.Size < 196608)
                {
                    if (batchSize < 196608)
                    {
                        batchSize += message.Size;
                        yield return message;
                    }
                    else
                    {
                        extraList.Value.Add(message);
                    }
                }
                else
                {
                    Logger.LogWarning($"Message too big: {message.Size} bytes, {message.ContentType}, skipping.");
                }
            }
        }
    }
}