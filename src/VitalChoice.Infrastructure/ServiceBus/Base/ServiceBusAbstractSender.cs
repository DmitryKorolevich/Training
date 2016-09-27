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
        protected volatile TQue Que;

        protected ServiceBusAbstractSender(Func<TQue> queFactory, ILogger logger)
        {
            QueFactory = queFactory;
            Que = queFactory();
            Logger = logger;
        }

        public void Dispose() => Que.Close();

        protected void DoSendAction<T>(Action<TQue, T> action, T value)
            where T : class
        {
            try
            {
                action(Que, value);
            }
            catch (OperationCanceledException)
            {
                action(Que, value);
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
                        action(Que, value);
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

        protected async Task DoSendActionAsync<T>(Func<TQue, T, Task> action, T value)
            where T : class
        {
            try
            {
                await action(Que, value);
            }
            catch (OperationCanceledException)
            {
                await action(Que, value);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                var oldQue = Que;
                Que = QueFactory();
                try
                {
                    await action(Que, value);
                }
                finally
                {
                    await oldQue.CloseAsync();
                }
            }
        }

        public abstract Task SendAsync(BrokeredMessage message);
        public abstract void Send(BrokeredMessage message);
        public abstract Task SendBatchAsync(IEnumerable<BrokeredMessage> messages);
        public abstract void SendBatch(IEnumerable<BrokeredMessage> messages);
    }
}