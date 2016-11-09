using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public abstract class ServiceBusAbstractSender<TQue> : IServiceBusSender
        where TQue : ClientEntity
    {
        protected readonly Func<TQue> QueFactory;
        protected readonly ILogger Logger;
        private readonly Func<object, BrokeredMessage> _messageConstructor;
        protected volatile TQue Que;

        protected ServiceBusAbstractSender(Func<TQue> queFactory, ILogger logger, Func<object, BrokeredMessage> messageConstructor)
        {
            QueFactory = queFactory;
            Que = queFactory();
            Logger = logger;
            _messageConstructor = messageConstructor;
        }

        public void Dispose() => Que.Close();

        protected void DoSendAction(Action<TQue, BrokeredMessage> action, object value)
        {
            try
            {
                action(Que, _messageConstructor(value));
            }
            catch (OperationCanceledException)
            {
                action(Que, _messageConstructor(value));
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                try
                {
                    var oldQue = Que;
                    Que = QueFactory();
                    try
                    {
                        action(Que, _messageConstructor(value));
                    }
                    finally
                    {
                        oldQue.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
        }

        protected async Task DoSendActionAsync(Func<TQue, BrokeredMessage, Task> action, object value)
        {
            try
            {
                await action(Que, _messageConstructor(value));
            }
            catch (OperationCanceledException)
            {
                await action(Que, _messageConstructor(value));
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                var oldQue = Que;
                Que = QueFactory();
                try
                {
                    await action(Que, _messageConstructor(value));
                }
                finally
                {
                    await oldQue.CloseAsync();
                }
            }
        }

        public abstract Task SendAsync(object message);
        public abstract void Send(object message);
        public abstract Task SendBatchAsync(IEnumerable<object> messages);
        public abstract void SendBatch(IEnumerable<object> messages);
    }
}