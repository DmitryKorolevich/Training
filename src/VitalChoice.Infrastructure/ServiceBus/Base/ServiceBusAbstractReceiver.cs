using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
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
        protected volatile bool Disposed;

        protected ServiceBusAbstractReceiver(Func<TQue> queFactory, ILogger logger)
        {
            QueFactory = queFactory;
            Que = queFactory();
            Logger = logger;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                Que.Close();
            }
        }

        protected virtual T DoReadAction<T>(Func<TQue, T> action)
            where T : class
        {
            try
            {
                return ReadResult(action);
            }
            catch (OperationCanceledException)
            {
                return ReadResult(action);
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
                        return ReadResult(action);
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

        private T ReadResult<T>(Func<TQue, T> action) where T : class => Disposed ? default(T) : action(Que);

        private Task<T> ReadResultAsync<T>(Func<TQue, Task<T>> action) where T : class
        => Disposed ? TaskCache<T>.DefaultCompletedTask : action(Que);

        protected virtual async Task<T> DoReadActionAsync<T>(Func<TQue, Task<T>> action)
            where T : class
        {
            try
            {
                return await ReadResultAsync(action);
            }
            catch (OperationCanceledException)
            {
                return await ReadResultAsync(action);
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
                        return await ReadResultAsync(action);
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