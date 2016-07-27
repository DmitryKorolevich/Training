#if !NETSTANDARD1_5
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public abstract class EncryptedServiceBusHost : IEncryptedServiceBusHost
    {
        private ServiceBusHostOneToMany _plainClient;
        private ServiceBusHostOneToMany _encryptedClient;
        private readonly ConcurrentDictionary<CommandItem, WeakReference<ServiceBusCommandBase>> _commands;

        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;
        public bool InitSuccess { get; }
        public virtual string LocalHostName { get; }
        public string ServerHostName { get; }

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger, IObjectEncryptionHost encryptionHost,
            IHostingEnvironment env)
        {
            ServerHostName = appOptions.Value.ExportService.ServerHostName;
            LocalHostName = env.ApplicationName + Guid.NewGuid().ToString("N");
            EncryptionHost = encryptionHost;
            Logger = logger;
            _commands = new ConcurrentDictionary<CommandItem, WeakReference<ServiceBusCommandBase>>();
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

        private void Initialize(IOptions<AppOptions> appOptions, ILogger logger)
        {
            EnsureTopicAndSubscriptionExists(appOptions.Value.ExportService.PlainConnectionString,
                appOptions.Value.ExportService.PlainQueueName, LocalHostName);
            EnsureTopicAndSubscriptionExists(appOptions.Value.ExportService.EncryptedConnectionString,
                appOptions.Value.ExportService.EncryptedQueueName, LocalHostName);

            _plainClient = new ServiceBusHostOneToMany(logger, () =>
            {
                var plainFactory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.PlainConnectionString);
                return plainFactory.CreateTopicClient(appOptions.Value.ExportService.PlainQueueName);
            }, () =>
            {
                var factory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.PlainConnectionString);
                return factory.CreateSubscriptionClient(appOptions.Value.ExportService.PlainQueueName, LocalHostName,
                    ReceiveMode.ReceiveAndDelete);
            }, m =>
            {
                try
                {
                    ProcessPlainMessage(m);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            });
            _encryptedClient = new ServiceBusHostOneToMany(logger, () =>
            {
                var plainFactory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.EncryptedConnectionString);
                return plainFactory.CreateTopicClient(appOptions.Value.ExportService.EncryptedQueueName);
            }, () =>
            {
                var factory = MessagingFactory.CreateFromConnectionString(appOptions.Value.ExportService.EncryptedConnectionString);
                return factory.CreateSubscriptionClient(appOptions.Value.ExportService.EncryptedQueueName, LocalHostName,
                    ReceiveMode.ReceiveAndDelete);
            }, m =>
            {
                try
                {
                    ProcessEncryptedMessage(m);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            });
            _plainClient.Start();
            _encryptedClient.Start();
        }

        private void EnsureTopicAndSubscriptionExists(string connectionString, string topicName, string subscriptionName)
        {
            var ns = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!ns.TopicExists(topicName))
            {
                TopicDescription topic = new TopicDescription(topicName)
                {
                    EnableExpress = true,
                    EnablePartitioning = true,
                    EnableBatchedOperations = true,
                    DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                    RequiresDuplicateDetection = false
                };

                ns.CreateTopic(topic);
            }
            if (!ns.SubscriptionExists(topicName, subscriptionName))
            {
                SubscriptionDescription subscription = new SubscriptionDescription(topicName, subscriptionName)
                {
                    EnableBatchedOperations = true,
                    DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                    RequiresSession = false,
                    EnableDeadLetteringOnFilterEvaluationExceptions = false,
                    EnableDeadLetteringOnMessageExpiration = false,
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(20),
                    MaxDeliveryCount = 1
                };
                ns.CreateSubscription(subscription);
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
            EncryptionHost.UnlockSession(command.SessionId);
            CommandItem commandItem = command;
            WeakReference<ServiceBusCommandBase> reference;
            _commands.TryRemove(commandItem, out reference);
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
            }
            else
            {
                Logger.LogInfo(cmd => $"{cmd.CommandName} received ({cmd.CommandId})", remoteCommand);
                Task.Run(() =>
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
                }).ConfigureAwait(false);
            }
        }

        private void ProcessPlainMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null || message.CorrelationId != LocalHostName)
            {
                return;
            }
            ServiceBusCommandBase remoteCommand;
            if (EncryptionHost.RsaVerifyWithConvert(message.GetBody<TransportCommandData>(), out remoteCommand))
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
                    Task.Run(() =>
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
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                SendPlainCommand(new ServiceBusCommandBase(remoteCommand, false));
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

#endif