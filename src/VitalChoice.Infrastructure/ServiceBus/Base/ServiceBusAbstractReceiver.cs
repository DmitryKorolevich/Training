using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public abstract class ServiceBusAbstractReceiver<TQue> : IServiceBusReceiver
        where TQue : ClientEntity
    {
        protected readonly Func<TQue> QueFactory;
        protected readonly ILogger Logger;
        protected volatile TQue Que;

        protected ServiceBusAbstractReceiver(Func<TQue> queFactory, ILogger logger)
        {
            QueFactory = queFactory;
            Que = queFactory();
            Logger = logger;
        }

        public void Dispose() => Que.Close();

        protected T DoReadAction<T>(Func<TQue, T> action)
            where T : class
        {
            try
            {
                return action(Que);
            }
            catch (OperationCanceledException)
            {
                return action(Que);
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
                        var result = action(Que);
                        return result;
                    }
                    finally
                    {
                        oldQue.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return null;
                }
            }
        }

        protected async Task<T> DoReadActionAsync<T>(Func<TQue, Task<T>> action)
            where T : class
        {
            try
            {
                return await action(Que);
            }
            catch (OperationCanceledException)
            {
                return await action(Que);
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
                        var result = await action(Que);
                        return result;
                    }
                    finally
                    {
                        await oldQue.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    return null;
                }
            }
        }

        public abstract Task<BrokeredMessage> ReceiveAsync();
        public abstract BrokeredMessage Receive();
        public abstract Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count);
        public abstract IEnumerable<BrokeredMessage> ReceiveBatch(int count);
    }
}