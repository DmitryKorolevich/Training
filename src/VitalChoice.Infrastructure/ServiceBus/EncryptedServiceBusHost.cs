﻿using System;
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
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public abstract class EncryptedServiceBusHost : IEncryptedServiceBusHost
    {
        
#if NET451 || DNX451
        private readonly QueueClient _plainClient;
        private readonly QueueClient _encryptedClient;
#endif
        private readonly ManualResetEvent _readyToDisposePlain;
        private volatile int _plainRunningCount;
        private readonly ManualResetEvent _readyToDisposeEncrypted;
        private volatile int _encryptedRunningCount;
        private readonly Dictionary<CommandItem, WeakReference<ServiceBusCommandBase>> _commands;

        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;
        public virtual string LocalHostName { get; }
        public string ServerHostName { get; }

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger, IObjectEncryptionHost encryptionHost)
        {
            ServerHostName = appOptions.Value.ExportService.ServerHostName;
            LocalHostName = Guid.NewGuid().ToString();
            EncryptionHost = encryptionHost;
            _readyToDisposePlain = new ManualResetEvent(true);
            _readyToDisposeEncrypted = new ManualResetEvent(true);
            Logger = logger;
            _commands = new Dictionary<CommandItem, WeakReference<ServiceBusCommandBase>>();
#if NET451 || DNX451
            try
            {
                _encryptedClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                    appOptions.Value.ExportService.EncryptedQueueName);
                _plainClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                    appOptions.Value.ExportService.PlainQueueName);
                ReceiveMessages();
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message, e);
            }
#endif
        }

        protected async Task SendPlainCommand(ServiceBusCommandBase command)
        {
#if NET451 || DNX451
            if (await SendPlainAsync(command))
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        protected async Task<T> ExecutePlainCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
#if NET451 || DNX451
            if (await SendPlainAsync(command))
#endif
            {
                Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
                return await Task.Run(() =>
                {
                    if (!command.ReadyEvent.WaitOne(command.TimeToLeave))
                    {
                        Logger.LogWarning($"Command timeout. <{command.CommandName}>({command.CommandId})");
                        CommandComplete(command);
                        return default(T);
                    }

                    CommandComplete(command);
                    if (command.Result == null)
                        return default(T);
                    return (T) command.Result;
                });
            }
            return default(T);
        }

        public async Task SendCommand(ServiceBusCommandBase command)
        {
#if NET451 || DNX451
            if (await SendEncryptedAsync(command))
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public async Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command)
        {
            command.OnComplete = CommandComplete;
            TrackCommand(command);
#if NET451 || DNX451
            if (await SendEncryptedAsync(command))
#endif
            {
                Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
                return await Task.Run(() =>
                {
                    if (!command.ReadyEvent.WaitOne(command.TimeToLeave))
                    {
                        Logger.LogWarning($"Command timeout. <{command.CommandName}>({command.CommandId})");
                        CommandComplete(command);
                        return default(T);
                    }
                    CommandComplete(command);
                    if (command.Result == null)
                        return default(T);
                    return (T) command.Result;
                });
            }
            return default(T);
        }

        public async Task ExecuteCommand(ServiceBusCommandBase command,
            Action<ServiceBusCommandBase, object> commandResultAction)
        {
            TrackCommand(command);
            command.RequestAcqureAction = commandResultAction;
#if NET451 || DNX451
            if (await SendEncryptedAsync(command))
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public bool IsAuthenticatedClient(Guid sessionId)
        {
            return EncryptionHost.SessionExist(sessionId);
        }

#if NET451 || DNX451
        protected virtual BrokeredMessage CreatePlainCommand(ServiceBusCommandBase command)
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

        protected virtual Task<bool> ProcessEncryptedCommand(ServiceBusCommandBase command)
        {
            return Task.FromResult(false);
        }

        protected virtual Task<bool> ProcessPlainCommand(ServiceBusCommandBase command)
        {
            return Task.FromResult(false);
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

        private void ReceiveMessages()
        {
#if NET451 || DNX451
            _plainClient.OnMessageAsync(ProcessPlainMessage, new OnMessageOptions {AutoComplete = false, MaxConcurrentCalls = 4});
            _encryptedClient.OnMessageAsync(ProcessEncryptedMessage, new OnMessageOptions {AutoComplete = false, MaxConcurrentCalls = 4});
#endif
        }

#if NET451 || DNX451
        private async Task ProcessEncryptedMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null)
            {
                await message.CompleteAsync();
                return;
            }
            if (message.CorrelationId != LocalHostName)
                return;
            try
            {
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
                    await message.CompleteAsync();
                }
                else
                {
                    Logger.LogInformation($"{remoteCommand.CommandName} received ({remoteCommand.CommandId})");

                    if (await ProcessEncryptedCommand(remoteCommand))
                    {
                        await message.CompleteAsync();
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

        private async Task ProcessPlainMessage(BrokeredMessage message)
        {
            if (message.CorrelationId == null)
            {
                await message.CompleteAsync();
                return;
            }
            if (message.CorrelationId != LocalHostName)
                return;
            try
            {
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
                        await message.CompleteAsync();
                    }
                    else
                    {
                        Logger.LogInformation($"{remoteCommand.CommandName} received ({remoteCommand.CommandId})");
                        if (await ProcessPlainCommand(remoteCommand))
                        {
                            await message.CompleteAsync();
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

        private Task<bool> SendEncryptedAsync(ServiceBusCommandBase command)
        {
            return Task.Run(() =>
            {
                if (_encryptedClient != null)
                {
                    lock (_encryptedClient)
                    {
                        _encryptedClient.SendAsync(CreateEncryptedMessage(command)).Wait();
                    }
                    return true;
                }
                return false;
            });
        }

        private Task<bool> SendPlainAsync(ServiceBusCommandBase command)
        {
            return Task.Run(() =>
            {
                if (_plainClient != null)
                {
                    lock (_plainClient)
                    {
                        _plainClient.SendAsync(CreatePlainCommand(command)).Wait();
                    }
                    return true;
                }
                return false;
            });
        }
#endif

        public virtual void Dispose()
        {
            WaitHandle.WaitAll(new WaitHandle[] {_readyToDisposeEncrypted, _readyToDisposePlain},
                TimeSpan.FromSeconds(20));
#if NET451 || DNX451
            _plainClient?.Close();
            _encryptedClient?.Close();
#endif
            _readyToDisposeEncrypted.Dispose();
            _readyToDisposePlain.Dispose();
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