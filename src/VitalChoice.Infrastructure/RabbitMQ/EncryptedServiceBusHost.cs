using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Crypto;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.RabbitMQ.Base;
using VitalChoice.Infrastructure.ServiceBus;

namespace VitalChoice.Infrastructure.RabbitMQ
{
    public abstract class EncryptedServiceBusHost : IEncryptedServiceBusHost
    {
        private ServiceBusReceiverHost _plainClient;
        private ServiceBusReceiverHost _encryptedClient;
        private ServiceBusTopicSender<ServiceBusCommandBase> _plainSender;
        private ServiceBusTopicSender<ServiceBusCommandBase> _encryptedSender;
        private SendingPool<ServiceBusCommandBase> _plainPool;
        private SendingPool<ServiceBusCommandBase> _encryptedPool;
        private readonly ConcurrentDictionary<CommandItem, WeakReference<ServiceBusCommandBase>> _commands;

        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;
        public bool Disabled { get; }
        public bool InitSuccess { get; }
        public virtual string LocalHostName { get; }
        public string ServerHostName { get; }

        public static int MaxProcessThreads { get; } = 10;

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger, IObjectEncryptionHost encryptionHost,
            IHostingEnvironment env)
        {
            Disabled = appOptions.Value.ExportService.Disabled;
            _commands = new ConcurrentDictionary<CommandItem, WeakReference<ServiceBusCommandBase>>();
            if (Disabled)
                return;
            ServerHostName = appOptions.Value.ExportService.ServerHostName;
            LocalHostName = env.ApplicationName + Guid.NewGuid().ToString("N");
            EncryptionHost = encryptionHost;
            Logger = logger;
            try
            {
                Initialize(appOptions, logger);
                InitSuccess = true;
            }
            catch (Exception e)
            {
                InitSuccess = false;
                logger.LogCritical(e.ToString());
            }
        }

        protected void SendPlainCommand(ServiceBusCommandBase command)
        {
            if (SendPlain(command))
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} sent ({cmd.CommandId})", command);
            }
        }

        protected async Task<T> ExecutePlainCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
            if (SendPlain(command))
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} sent ({cmd.CommandId})", command);
                if (!await command.ReadyEvent.WaitAsync(command.TimeToLeave))
                {
                    Logger.LogWarning($"Command timeout. <{command.CommandName}>({command.CommandId})");
                    CommandComplete(command);
                    return default(T);
                }

                CommandComplete(command);
                if (!string.IsNullOrEmpty(command.Result.Error))
                {
                    throw new ApiException(command.Result.Error);
                }
                if (command.Result.Data is T)
                {
                    return (T) command.Result.Data;
                }
                Logger.LogWarning($"Cannot cast {command.Result.Data?.GetType()} to {typeof(T)}");
                return default(T);
            }
            return default(T);
        }

        protected virtual BrokeredMessage CreatePlainMessage(ServiceBusCommandBase command)
        {
            return new BrokeredMessage(EncryptionHost.SignWithConvert(command))
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

        public void SendCommand(ServiceBusCommandBase command)
        {
            if (SendEncrypted(command))
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} sent ({cmd.CommandId})", command);
            }
        }

        public async Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
            if (SendEncrypted(command))
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} sent ({cmd.CommandId})", command);
                if (!await command.ReadyEvent.WaitAsync(command.TimeToLeave))
                {
                    Logger.LogWarning($"Command timeout. <{command.CommandName}>({command.CommandId})");
                    CommandComplete(command);
                    return default(T);
                }
                CommandComplete(command);
                if (!string.IsNullOrEmpty(command.Result.Error))
                {
                    throw new ApiException(command.Result.Error);
                }
                if (command.Result.Data is T)
                {
                    return (T) command.Result.Data;
                }
                Logger.LogWarning($"Cannot cast {command.Result.Data?.GetType()} to {typeof(T)}");
                return default(T);
            }
            return default(T);
        }

        public void ExecuteCommand(ServiceBusCommandBase command,
            Action<ServiceBusCommandBase, ServiceBusCommandData> commandResultAction)
        {
            TrackCommand(command);
            command.RequestAcqureAction = commandResultAction;
            if (SendEncrypted(command))
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} sent ({cmd.CommandId})", command);
            }
        }

        private void CommandComplete(ServiceBusCommandBase command)
        {
            CommandItem commandItem = command;
            WeakReference<ServiceBusCommandBase> reference;
            _commands.TryRemove(commandItem, out reference);
        }

        public void Initialize(IOptions<AppOptions> appOptions, ILogger logger)
        {
            var subscriptionPlainFactory = new SubcriptionDefaultFactory(appOptions.Value.ExportService.PlainConnectionString,
                appOptions.Value.ExportService.PlainQueueName, LocalHostName, ReceiveMode.ReceiveAndDelete);

            var subscriptionEncryptedFactory = new SubcriptionDefaultFactory(appOptions.Value.ExportService.EncryptedConnectionString,
                appOptions.Value.ExportService.EncryptedQueueName, LocalHostName, ReceiveMode.ReceiveAndDelete);

            _plainClient = new ServiceBusReceiverHost(logger, new ServiceBusSubscriptionReceiver(subscriptionPlainFactory.Create, logger));
            _plainClient.ReceiveEvent += ProcessPlainMessage;

            _encryptedClient = new ServiceBusReceiverHost(logger,
                new ServiceBusSubscriptionReceiver(subscriptionEncryptedFactory.Create, logger));
            _encryptedClient.ReceiveEvent += ProcessEncryptedMessage;

            var topicPlainFactory = new TopicDefaultFactory(appOptions.Value.ExportService.PlainConnectionString,
                appOptions.Value.ExportService.PlainQueueName);

            var topicEncryptedFactory = new TopicDefaultFactory(appOptions.Value.ExportService.EncryptedConnectionString,
                appOptions.Value.ExportService.EncryptedQueueName);

            _plainSender = new ServiceBusTopicSender<ServiceBusCommandBase>(topicPlainFactory.Create, logger, CreatePlainMessage);
            _encryptedSender = new ServiceBusTopicSender<ServiceBusCommandBase>(topicEncryptedFactory.Create, logger, CreateEncryptedMessage);

            _plainPool = new SendingPool<ServiceBusCommandBase>(_plainSender, logger);
            _encryptedPool = new SendingPool<ServiceBusCommandBase>(_encryptedSender, logger);

            _plainClient.Start();
            _encryptedClient.Start();
        }

        private void ProcessEncryptedMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null || message.CorrelationId != LocalHostName)
            {
                return;
            }

            var remoteCommand = EncryptionHost.AesDecryptVerify<ServiceBusCommandBase>(message.GetBody<TransportCommandData>(),
                Guid.Parse(message.SessionId));

            if (remoteCommand == null)
            {
                Logger.LogWarning($"Invalid sign. Session: {message.SessionId}");
                return;
            }
            WeakReference<ServiceBusCommandBase> commandReference;
            _commands.TryGetValue(remoteCommand, out commandReference);
            if (commandReference != null)
            {
                ServiceBusCommandBase command;
                if (commandReference.TryGetTarget(out command))
                {
                    Logger.LogInfo(cmd => $"{cmd.CommandName} answer received ({cmd.CommandId})", command);
                    command.RequestAcqureAction?.Invoke(command, remoteCommand.Data);
                }
                else
                {
                    Logger.LogWarn(cmd => $"{cmd.CommandName} ({cmd.CommandId}) Reference Removed before answer received", remoteCommand);
                }
            }
            else
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} received ({cmd.CommandId})", remoteCommand);
                ProcessEncryptedRemoteCommand(remoteCommand);
            }
        }

        private void ProcessPlainMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null || message.CorrelationId != LocalHostName)
            {
                return;
            }
            ServiceBusCommandBase remoteCommand;
            if (EncryptionHost.VerifySignWithConvert(message.GetBody<TransportCommandData>(), out remoteCommand))
            {
                WeakReference<ServiceBusCommandBase> commandReference;
                _commands.TryGetValue(remoteCommand, out commandReference);

                if (commandReference != null)
                {
                    ServiceBusCommandBase command;
                    if (commandReference.TryGetTarget(out command))
                    {
                        Logger.LogInfo(cmd => $"{cmd.CommandName} answer received ({cmd.CommandId})", command);
                        command.RequestAcqureAction?.Invoke(command, remoteCommand.Data);
                    }
                }
                else
                {
                    Logger.LogInfo(cmd => $"{cmd.CommandName} received ({cmd.CommandId})", remoteCommand);
                    ProcessPlainRemoteCommand(remoteCommand);
                }
            }
            else
            {
                if (remoteCommand != null)
                {
                    SendPlainCommand(new ServiceBusCommandBase(remoteCommand, false));
                    Logger.LogWarning($"Invalid Sign. Command: {remoteCommand.CommandName} Data: {remoteCommand.Data?.Data:X2}");
                }
            }
        }

        private void ProcessEncryptedRemoteCommand(ServiceBusCommandBase remoteCommand)
        {
            try
            {
                if (ProcessEncryptedCommand(remoteCommand))
                {
                    Logger.LogInfo(cmd => $"{cmd.CommandName} processing success ({cmd.CommandId})", remoteCommand);
                }
                else
                {
                    Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                SendEncrypted(new ServiceBusCommandBase(remoteCommand, e.ToString()));
            }
        }

        private void ProcessPlainRemoteCommand(ServiceBusCommandBase remoteCommand)
        {
            try
            {
                if (ProcessPlainCommand(remoteCommand))
                {
                    Logger.LogInfo(cmd => $"{cmd.CommandName} processing success ({cmd.CommandId})", remoteCommand);
                }
                else
                {
                    Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                Logger.LogWarning($"{remoteCommand.CommandName} processing failed ({remoteCommand.CommandId})");
                SendPlainCommand(new ServiceBusCommandBase(remoteCommand, e.ToString()));
            }
        }

        private bool SendEncrypted(ServiceBusCommandBase command)
        {
            if (_encryptedClient == null)
                return false;

            _encryptedPool.EnqueueData(command);
            return true;
        }

        private bool SendPlain(ServiceBusCommandBase command)
        {
            if (_plainClient == null)
                return false;

            _plainPool.EnqueueData(command);
            return true;
        }

        public virtual void Dispose()
        {
            _plainClient?.Dispose();
            _encryptedClient?.Dispose();
            EncryptionHost?.Dispose();
            _plainPool?.Dispose();
            _encryptedPool?.Dispose();
        }

        private void TrackCommand(ServiceBusCommandBase command)
        {
            _commands.TryAdd(command, new WeakReference<ServiceBusCommandBase>(command));
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
                return obj is CommandItem && Equals((CommandItem) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_sessionId.GetHashCode()*397) ^ _commandId.GetHashCode();
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