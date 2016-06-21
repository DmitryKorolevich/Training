#if !NETSTANDARD1_5
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Extensions;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public abstract class EncryptedServiceBusHost : IEncryptedServiceBusHost
    {
        private readonly ServiceBusHostOneToOne _plainClient;
        private readonly ServiceBusHostOneToOne _encryptedClient;
        private readonly ConcurrentDictionary<CommandItem, WeakReference<ServiceBusCommandBase>> _commands;

        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;
        public virtual string LocalHostName { get; }
        public string ServerHostName { get; }

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger, IObjectEncryptionHost encryptionHost)
        {
            ServerHostName = appOptions.Value.ExportService.ServerHostName;
            LocalHostName = Guid.NewGuid().ToString();
            EncryptionHost = encryptionHost;
            Logger = logger;
            _commands = new ConcurrentDictionary<CommandItem, WeakReference<ServiceBusCommandBase>>();
            try
            {
                _plainClient = new ServiceBusHostOneToOne(logger, () =>
                {
                    var plainFactory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.PlainConnectionString);
                    return plainFactory.CreateQueueClient(appOptions.Value.ExportService.PlainQueueName, ReceiveMode.PeekLock);
                });
                _encryptedClient = new ServiceBusHostOneToOne(logger, () =>
                {
                    var encryptedFactory =
                        MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.EncryptedConnectionString);
                    return encryptedFactory.CreateQueueClient(appOptions.Value.ExportService.EncryptedQueueName, ReceiveMode.PeekLock);
                });

                _plainClient.ReceiveMessagesEvent += messages => messages.ForEach(ProcessPlainMessage);
                _plainClient.Start();

                _encryptedClient.ReceiveMessagesEvent += messages => messages.ForEach(ProcessEncryptedMessage);
                _encryptedClient.Start();
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message, e);
            }
        }

        protected void SendPlainCommand(ServiceBusCommandBase command)
        {
            if (SendPlain(command))
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        protected async Task<T> ExecutePlainCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
            if (SendPlain(command))
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
            if (SendEncrypted(command))
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public async Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
            if (SendEncrypted(command))
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
            if (SendEncrypted(command))
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public bool IsAuthenticatedClient(Guid sessionId)
        {
            return EncryptionHost.SessionExist(sessionId);
        }
        
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
                WeakReference<ServiceBusCommandBase> reference;
                _commands.TryRemove(commandItem, out reference);
            }
        }

        private void ProcessEncryptedMessage(BrokeredMessage message)
        {
            if (message.CorrelationId != LocalHostName)
            {
                message.Abandon();
                return;
            }
            if (message.ExpiresAtUtc < DateTime.UtcNow)
            {
                message.Complete();
                return;
            }
            if (message.CorrelationId == null)
            {
                message.Complete();
                return;
            }
            message.Complete();

            var remoteCommand = EncryptionHost.AesDecryptVerify<ServiceBusCommandBase>(message.GetBody<TransportCommandData>(),
                Guid.Parse(message.SessionId));

            if (remoteCommand == null)
            {
                Logger.LogWarning($"Invalid sign. Session: {message.SessionId}");
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
                Task.Run(() =>
                {
                    if (ProcessEncryptedCommand(remoteCommand))
                    {
                        Logger.LogInformation($"{remoteCommand.CommandName} processing success ({remoteCommand.CommandId})");
                    }
                    else
                    {
                        Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                    }
                }).ConfigureAwait(false);
            }
        }

        private void ProcessPlainMessage(BrokeredMessage message)
        {
            if (message.CorrelationId != LocalHostName)
            {
                message.Abandon();
                return;
            }
            if (message.ExpiresAtUtc < DateTime.UtcNow)
            {
                message.Complete();
                return;
            }
            if (message.CorrelationId == null)
            {
                message.Complete();
                return;
            }
            message.Complete();
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
                    Task.Run(() =>
                    {
                        if (ProcessPlainCommand(remoteCommand))
                        {
                            Logger.LogInformation($"{remoteCommand.CommandName} processing success ({remoteCommand.CommandId})");
                        }
                        else
                        {
                            Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                        }
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                Logger.LogWarning($"Invalid Sign. Message: {message.MessageId}");
            }
        }

        private bool SendEncrypted(ServiceBusCommandBase command)
        {
            if (_encryptedClient == null)
                return false;

            _encryptedClient.EnqueueMessage(CreateEncryptedMessage(command));
            _encryptedClient.SendNow();
            return true;
        }

        private bool SendPlain(ServiceBusCommandBase command)
        {
            if (_plainClient == null)
                return false;

            _plainClient.EnqueueMessage(CreatePlainMessage(command));
            _plainClient.SendNow();
            return true;
        }

        public virtual void Dispose()
        {
            _plainClient?.Dispose();
            _encryptedClient?.Dispose();
            EncryptionHost?.Dispose();
        }

        private void TrackCommand(ServiceBusCommandBase command)
        {
            lock (_commands)
            {
                _commands.TryAdd(command, new WeakReference<ServiceBusCommandBase>(command));
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
                return _sessionId.Equals(other._sessionId) && _commandId.Equals(other._commandId);
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
#endif