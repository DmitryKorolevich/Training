using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
#if NET451 || DNX451
using Microsoft.ServiceBus.Messaging;
#endif
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Helpers.Async;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public abstract class EncryptedServiceBusHost : IEncryptedServiceBusHost
    {
        
#if NET451 || DNX451
        private readonly QueueClient _plainClient;
        private readonly QueueClient _encryptedClient;
        private readonly ConcurrentQueue<BrokeredMessage> _sendQue;
#endif
        private readonly ManualResetEvent _readyToDisposePlain;
        private readonly ManualResetEvent _readyToDisposeEncrypted;
        private volatile int _plainRunningCount;
        private volatile int _encryptedRunningCount;
        private readonly AutoResetEvent _newMessageSent;
        private readonly Dictionary<CommandItem, WeakReference<ServiceBusCommandBase>> _commands;
        private volatile bool _terminated;

        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;
        public virtual string LocalHostName { get; }
        public string ServerHostName { get; }

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger, IObjectEncryptionHost encryptionHost)
        {
            ServerHostName = appOptions.Value.ExportService.ServerHostName;
            LocalHostName = Guid.NewGuid().ToString();
            EncryptionHost = encryptionHost;
            _newMessageSent = new AutoResetEvent(false);
            _readyToDisposePlain = new ManualResetEvent(true);
            _readyToDisposeEncrypted = new ManualResetEvent(true);
            Logger = logger;
            _commands = new Dictionary<CommandItem, WeakReference<ServiceBusCommandBase>>();
#if NET451 || DNX451
            try
            {
                _sendQue = new ConcurrentQueue<BrokeredMessage>();
                var plainFactory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.PlainConnectionString);
                var encryptedFactory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.EncryptedConnectionString);

                _encryptedClient = plainFactory.CreateQueueClient(appOptions.Value.ExportService.EncryptedQueueName, ReceiveMode.PeekLock);
                _plainClient = encryptedFactory.CreateQueueClient(appOptions.Value.ExportService.PlainQueueName, ReceiveMode.PeekLock);
                new Thread(ReceivePlainMessages).Start();
                new Thread(ReceiveEncryptedMessages).Start();

                //Make two send threads
                new Thread(SendMessages).Start();
                new Thread(SendMessages).Start();
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message, e);
            }
#endif
        }

        protected void SendPlainCommand(ServiceBusCommandBase command)
        {
#if NET451 || DNX451
            if (SendPlain(command))
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        protected async Task<T> ExecutePlainCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
#if NET451 || DNX451
            if (SendPlain(command))
#endif
            {
                Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
                if (!await command.ReadyEvent.WaitAsync(command.TimeToLeave))
                {
                    Logger.LogWarning($"Command timeout. <{command.CommandName}>({command.CommandId})");
                    CommandComplete(command);
                    return default(T);
                }

                CommandComplete(command);
                if (command.Result == null)
                    return default(T);
                return (T) command.Result;
            }
            return default(T);
        }

        public void SendCommand(ServiceBusCommandBase command)
        {
#if NET451 || DNX451
            if (SendEncrypted(command))
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public async Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
#if NET451 || DNX451
            if (SendEncrypted(command))
#endif
            {
                Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
                if (!await command.ReadyEvent.WaitAsync(command.TimeToLeave))
                {
                    Logger.LogWarning($"Command timeout. <{command.CommandName}>({command.CommandId})");
                    CommandComplete(command);
                    return default(T);
                }
                CommandComplete(command);
                if (command.Result == null)
                    return default(T);
                return (T) command.Result;
            }
            return default(T);
        }

        public void ExecuteCommand(ServiceBusCommandBase command,
            Action<ServiceBusCommandBase, object> commandResultAction)
        {
            TrackCommand(command);
            command.RequestAcqureAction = commandResultAction;
#if NET451 || DNX451
            if (SendEncrypted(command))
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public bool IsAuthenticatedClient(Guid sessionId)
        {
            return EncryptionHost.SessionExist(sessionId);
        }

#if NET451 || DNX451
        protected virtual BrokeredMessage CreatePlainMessage(ServiceBusCommandBase command)
        {
            return new BrokeredMessage(EncryptionHost.RsaSignWithConvert(command))
            {
                TimeToLive = command.TimeToLeave,
                CorrelationId = command.Destination
            };
        }

        protected virtual BrokeredMessage CreateEncryptedMessage(ServiceBusCommandBase command)
        {
            return new BrokeredMessage(EncryptionHost.AesEncryptSign(command, command.SessionId))
            {
                SessionId = command.SessionId.ToString(),
                TimeToLive = command.TimeToLeave,
                CorrelationId = command.Destination
            };
        }
#endif

        protected virtual bool ProcessEncryptedCommand(ServiceBusCommandBase command)
        {
            return false;
        }

        protected virtual bool ProcessPlainCommand(ServiceBusCommandBase command)
        {
            return false;
        }

        private void CommandComplete(ServiceBusCommandBase command)
        {
            lock (_commands)
            {
                CommandItem commandItem = command;
                if (_commands.ContainsKey(commandItem))
                {
                    _commands.Remove(commandItem);
                }
            }
        }

        private void ReceivePlainMessages()
        {
#if NET451 || DNX451
            while (!_terminated)
            {
                var message = _plainClient.Receive();
                if (message != null)
                    ProcessPlainMessage(message);
            }
#endif
        }

        private void ReceiveEncryptedMessages()
        {
#if NET451 || DNX451
            while (!_terminated)
            {
                var message = _encryptedClient.Receive();
                if (message != null)
                    ProcessEncryptedMessage(message);
            }
#endif
        }

        private void SendMessages()
        {
#if NET451 || DNX451
            while (!_terminated)
            {
                BrokeredMessage message;
                if (!_sendQue.TryDequeue(out message))
                {
                    _newMessageSent.WaitOne();
                    continue;
                }
                if (message.SessionId != null)
                {
                    _encryptedClient.Send(message);
                }
                else
                {
                    _plainClient.Send(message);
                }
            }
#endif
        }

#if NET451 || DNX451
        private void ProcessEncryptedMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null)
            {
                message.Complete();
                return;
            }
            if (message.CorrelationId != LocalHostName)
            {
                message.Abandon();
                return;
            }
            try
            {
                message.Complete();
                _plainRunningCount++;
                if (_plainRunningCount == 1)
                    _readyToDisposeEncrypted.Reset();

                var remoteCommand = EncryptionHost.AesDecryptVerify<ServiceBusCommandBase>(message.GetBody<TransportCommandData>(),
                    Guid.Parse(message.SessionId));

                if (remoteCommand == null)
                {
                    Logger.LogInformation($"Invalid sign. Session: {message.SessionId}");
                    return;
                }
                WeakReference<ServiceBusCommandBase> commandReference;
                lock (_commands)
                {
                    _commands.TryGetValue(remoteCommand, out commandReference);
                }
                if (commandReference != null)
                {
                    ServiceBusCommandBase command;
                    if (commandReference.TryGetTarget(out command))
                    {
                        Logger.LogInformation($"{command.CommandName} answer received ({command.CommandId})");
                        command.RequestAcqureAction?.Invoke(command, remoteCommand.Data);
                    }
                }
                else
                {
                    Logger.LogInformation($"{remoteCommand.CommandName} received ({remoteCommand.CommandId})");

                    if (ProcessEncryptedCommand(remoteCommand))
                    {
                        Logger.LogInformation($"{remoteCommand.CommandName} processing success ({remoteCommand.CommandId})");
                    }
                    else
                    {
                        Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("<ProcessEncryptedMessage> Error while processing incoming message", e);
                throw;
            }
            finally
            {
                _plainRunningCount--;
                if (_plainRunningCount == 0)
                    _readyToDisposeEncrypted.Set();
            }
        }

        private void ProcessPlainMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null)
            {
                message.Complete();
                return;
            }
            if (message.CorrelationId != LocalHostName)
            {
                message.Abandon();
                return;
            }
            try
            {
                message.Complete();
                _encryptedRunningCount++;
                if (_encryptedRunningCount == 1)
                    _readyToDisposePlain.Reset();
                ServiceBusCommandBase remoteCommand;
                if (EncryptionHost.RsaVerifyWithConvert(message.GetBody<TransportCommandData>(), out remoteCommand))
                {
                    WeakReference<ServiceBusCommandBase> commandReference;
                    lock (_commands)
                    {
                        _commands.TryGetValue(remoteCommand,
                            out commandReference);
                    }

                    if (commandReference != null)
                    {
                        ServiceBusCommandBase command;
                        if (commandReference.TryGetTarget(out command))
                        {
                            Logger.LogInformation($"{command.CommandName} answer received ({command.CommandId})");
                            command.RequestAcqureAction?.Invoke(command, remoteCommand.Data);
                        }
                    }
                    else
                    {
                        Logger.LogInformation($"{remoteCommand.CommandName} received ({remoteCommand.CommandId})");
                        if (ProcessPlainCommand(remoteCommand))
                        {
                            Logger.LogInformation($"{remoteCommand.CommandName} processing success ({remoteCommand.CommandId})");
                        }
                        else
                        {
                            Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                        }
                    }
                }
                else
                {
                    throw new AccessDeniedException("Invalid sign");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("<ProcessPlainMessage> Error while processing incoming message", e);
                throw;
            }
            finally
            {
                _encryptedRunningCount--;
                if (_encryptedRunningCount == 0)
                    _readyToDisposePlain.Set();
            }
        }

        private bool SendEncrypted(ServiceBusCommandBase command)
        {
            if (_encryptedClient == null)
                return false;

            _sendQue.Enqueue(CreateEncryptedMessage(command));
            _newMessageSent.Set();
            return true;
        }

        private bool SendPlain(ServiceBusCommandBase command)
        {
            if (_plainClient == null)
                return false;

            _sendQue.Enqueue(CreatePlainMessage(command));
            _newMessageSent.Set();
            return true;
        }
#endif

        public virtual void Dispose()
        {
            _terminated = true;
#if NET451 || DNX451
            _plainClient?.Close();
            _encryptedClient?.Close();
#endif
            WaitHandle.WaitAll(new WaitHandle[] {_readyToDisposeEncrypted, _readyToDisposePlain}, TimeSpan.FromSeconds(20));
            EncryptionHost?.Dispose();
        }

        private void TrackCommand(ServiceBusCommandBase command)
        {
            lock (_commands)
            {
                _commands.Add(command, new WeakReference<ServiceBusCommandBase>(command));
            }
        }

        private struct CommandItem : IEquatable<CommandItem>
        {
            private CommandItem(Guid commandId, Guid sessionId)
            {
                _commandId = commandId;
                _sessionId = sessionId;
            }

            public bool Equals(CommandItem other)
            {
                return string.Equals(_sessionId, other._sessionId) && _commandId.Equals(other._commandId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is CommandItem && Equals((CommandItem)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_sessionId.GetHashCode() * 397) ^ _commandId.GetHashCode();
                }
            }

            public static implicit operator CommandItem(ServiceBusCommandBase command)
            {
                if (command == null)
                    throw new ArgumentNullException(nameof(command));
                return new CommandItem(command.CommandId, command.SessionId);
            }

            private readonly Guid _sessionId;
            private readonly Guid _commandId;
        }
    }
}