using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public abstract class ServiceBusAbstractSender<TQue, T> : IServiceBusSender<T>
        where TQue : ClientEntity
    {
        protected readonly Func<TQue> QueFactory;
        protected readonly ILogger Logger;
        private readonly Func<T, BrokeredMessage> _messageConstructor;
        protected volatile TQue Que;
        protected volatile bool Disposed;

        protected ServiceBusAbstractSender(Func<TQue> queFactory, ILogger logger, Func<T, BrokeredMessage> messageConstructor)
        {
            QueFactory = queFactory;
            Que = queFactory();
            Logger = logger;
            _messageConstructor = messageConstructor;
        }

        public virtual void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                Que.Close();
            }
        }

        protected virtual void DoSendAction(Action<TQue, BrokeredMessage> action, T value)
        {
            try
            {
                ExecuteSingleWithCheck(action, value);
            }
            catch (OperationCanceledException)
            {
                ExecuteSingleWithCheck(action, value);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                Thread.Sleep(TimeSpan.FromSeconds(10));
                try
                {
                    var oldQue = Que;
                    Que = QueFactory();
                    try
                    {
                        ExecuteSingleWithCheck(action, value);
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

        protected virtual void DoCollectionSendAction(Action<TQue, IEnumerable<BrokeredMessage>> action, ICollection<T> values)
        {
            try
            {
                ExecuteBatchWithCheck(action, values.Select(_messageConstructor));
            }
            catch (OperationCanceledException)
            {
                ExecuteBatchWithCheck(action, values.Select(_messageConstructor));
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                Thread.Sleep(TimeSpan.FromSeconds(10));
                try
                {
                    var oldQue = Que;
                    Que = QueFactory();
                    try
                    {
                        ExecuteBatchWithCheck(action, values.Select(_messageConstructor));
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

        protected virtual async Task DoSendActionAsync(Func<TQue, BrokeredMessage, Task> action, T value)
        {
            try
            {
                await ExecuteSingleWithCheckAsync(action, value);
            }
            catch (OperationCanceledException)
            {
                await ExecuteSingleWithCheckAsync(action, value);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                Thread.Sleep(TimeSpan.FromSeconds(10));
                try
                {
                    var oldQue = Que;
                    Que = QueFactory();
                    try
                    {
                        await ExecuteSingleWithCheckAsync(action, value);
                    }
                    finally
                    {
                        await oldQue.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
        }

        protected virtual async Task DoCollectionSendActionAsync(Func<TQue, IEnumerable<BrokeredMessage>, Task> action,
            ICollection<T> values)
        {
            try
            {
                await ExecuteBatchWithCheckAsync(action, values.Select(_messageConstructor));
            }
            catch (OperationCanceledException)
            {
                await ExecuteBatchWithCheckAsync(action, values.Select(_messageConstructor));
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
                Thread.Sleep(TimeSpan.FromSeconds(10));
                try
                {
                    var oldQue = Que;
                    Que = QueFactory();
                    try
                    {
                        await ExecuteBatchWithCheckAsync(action, values.Select(_messageConstructor));
                    }
                    finally
                    {
                        await oldQue.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
        }

        public abstract Task SendAsync(T message);
        public abstract void Send(T message);
        public abstract Task SendBatchAsync(ICollection<T> messages);
        public abstract void SendBatch(ICollection<T> messages);

        protected virtual async Task ExecuteSingleWithCheckAsync(Func<TQue, BrokeredMessage, Task> action, T value)
        {
            if (Disposed)
                return;

            var message = _messageConstructor(value);
            if (message.Size < 196608)
            {
                await action(Que, message);
            }
            else
            {
                Logger.LogWarning($"Message too big: {message.Size} bytes, {message.ContentType}");
            }
        }

        protected virtual void ExecuteSingleWithCheck(Action<TQue, BrokeredMessage> action, T value)
        {
            if (Disposed)
                return;

            var message = _messageConstructor(value);
            if (message.Size < 196608)
            {
                action(Que, message);
            }
            else
            {
                Logger.LogWarning($"Message too big: {message.Size} bytes, {message.ContentType}");
            }
        }

        protected virtual void ExecuteBatchWithCheck(Action<TQue, IEnumerable<BrokeredMessage>> action, IEnumerable<BrokeredMessage> values)
        {
            if (Disposed)
                return;

            var extraList = new Lazy<List<BrokeredMessage>>(() => new List<BrokeredMessage>(), LazyThreadSafetyMode.None);
            var messages = GetCompleteBatch(values, extraList);
            action(Que, messages);
            while (extraList.IsValueCreated)
            {
                var newExtraList = new Lazy<List<BrokeredMessage>>(() => new List<BrokeredMessage>(), LazyThreadSafetyMode.None);
                messages = GetCompleteBatch(extraList.Value, newExtraList);
                action(Que, messages);
                extraList = newExtraList;
            }
        }

        protected virtual async Task ExecuteBatchWithCheckAsync(Func<TQue, IEnumerable<BrokeredMessage>, Task> action, IEnumerable<BrokeredMessage> values)
        {
            if (Disposed)
                return;

            var extraList = new Lazy<List<BrokeredMessage>>(() => new List<BrokeredMessage>(), LazyThreadSafetyMode.None);
            var messages = GetCompleteBatch(values, extraList);
            await action(Que, messages);
            while (extraList.IsValueCreated)
            {
                var newExtraList = new Lazy<List<BrokeredMessage>>(() => new List<BrokeredMessage>(), LazyThreadSafetyMode.None);
                messages = GetCompleteBatch(extraList.Value, newExtraList);
                await action(Que, messages);
                extraList = newExtraList;
            }
        }

        protected virtual List<BrokeredMessage> GetCompleteBatch(IEnumerable<BrokeredMessage> messages,
            Lazy<List<BrokeredMessage>> extraList)
        {
            var results = new List<BrokeredMessage>(300);
            long batchSize = 0;
            foreach (var message in messages)
            {
                if (message.Size < 196608)
                {
                    if (batchSize < 196608)
                    {
                        batchSize += message.Size;
                        results.Add(message);
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
            return results;
        }
    }
}