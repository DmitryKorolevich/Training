﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.LoadBalancing;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class BatchSendingPool<T> : RoundRobinAbstractPool<ICollection<T>>
    {
        private readonly IServiceBusSender<T> _sender;

        public BatchSendingPool(IServiceBusSender<T> sender, ILogger logger) : base(1, logger)
        {
            _sender = sender;
        }

        protected override void ProcessingAction(ICollection<T> data, object localData, object processParameter)
        {
            _sender.SendBatch(data);
        }
    }
}