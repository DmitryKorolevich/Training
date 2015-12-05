using System;
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
        protected virtual string LocalHostName { get; }
        protected string ServerHostName { get; }

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

        protected async Task<T> ExecutePlainCommand<T>(ServiceBusCommand command)
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

        public async Task<T> ExecuteCommand<T>(ServiceBusCommand command)
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
            return new BrokeredMessage(EncryptionHost.RsaSignWithConvert(command.Data))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave,
                Label = LocalHostName,
                CorrelationId = command.Destination
            };
        }

        protected virtual BrokeredMessage CreateEncryptedMessage(ServiceBusCommandBase command)
        {
            return new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave,
                Label = LocalHostName,
                CorrelationId = command.Destination
            };
        }
#endif

        protected virtual Task<bool> ProcessEncryptedCommand(ServiceBusCommand command)
        {
            return Task.FromResult(false);
        }

        protected virtual Task<bool> ProcessPlainCommand(ServiceBusCommand command)
        {
            return Task.FromResult(false);
        }

        private void CommandComplete(ServiceBusCommandBase command)
        {
            lock (_commands)
            {
                var commandItem = new CommandItem(command.CommandId, command.CommandName);
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

                WeakReference<ServiceBusCommandBase> commandReference;
                lock (_commands)
                {
                    _commands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType),
                        out commandReference);
                }
                if (commandReference != null)
                {
                    ServiceBusCommandBase command;
                    if (commandReference.TryGetTarget(out command))
                    {
                        Logger.LogInformation($"{command.CommandName} intercepting ({command.CommandId})");
                        var body = EncryptionHost.AesDecrypt<object>(message.GetBody<SymmetricEncryptedCommandData>(), command.SessionId);
                        command.RequestAcqureAction?.Invoke(command, body);
                    }
                    await message.CompleteAsync();
                }
                else
                {
                    var incomingCommand = CreateIncomingCommand(message);
                    Logger.LogInformation($"{incomingCommand.CommandName} received ({incomingCommand.CommandId})");

                    var body = EncryptionHost.AesDecrypt<object>(message.GetBody<SymmetricEncryptedCommandData>(), incomingCommand.SessionId);
                    incomingCommand.RequestAcqureAction?.Invoke(incomingCommand, body);
                    if (await ProcessEncryptedCommand(incomingCommand))
                    {
                        await message.CompleteAsync();
                        Logger.LogInformation($"{incomingCommand.CommandName} processing success ({incomingCommand.CommandId})");
                    }
                    else
                    {
                        Logger.LogWarning($"{incomingCommand.CommandName} processing failed ({incomingCommand.CommandId})");
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

                WeakReference<ServiceBusCommandBase> commandReference;
                lock (_commands)
                {
                    _commands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType),
                        out commandReference);
                }
                object body;
                if (EncryptionHost.RsaCheckSignWithConvert(message.GetBody<PlainCommandData>(), out body))
                {
                    Logger.LogInformation($"{message.ContentType} incoming command identity validate sucess.");
                    if (commandReference != null)
                    {
                        ServiceBusCommandBase command;
                        if (commandReference.TryGetTarget(out command))
                        {
                            Logger.LogInformation($"{command.CommandName} intercepting ({command.CommandId})");
                            command.RequestAcqureAction?.Invoke(command, body);
                        }
                        await message.CompleteAsync();
                    }
                    else
                    {
                        var incomingCommand = CreateIncomingCommand(message);
                        Logger.LogInformation($"{incomingCommand.CommandName} received ({incomingCommand.CommandId})");
                        incomingCommand.RequestAcqureAction?.Invoke(incomingCommand, body);
                        if (await ProcessPlainCommand(incomingCommand))
                        {
                            await message.CompleteAsync();
                            Logger.LogInformation($"{incomingCommand.CommandName} processing success ({incomingCommand.CommandId})");
                        }
                        else
                        {
                            Logger.LogWarning($"{incomingCommand.CommandName} processing failed ({incomingCommand.CommandId})");
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

        protected virtual ServiceBusCommand CreateIncomingCommand(BrokeredMessage message)
        {
            return new ServiceBusCommand(Guid.Parse(message.SessionId), message.ContentType, message.CorrelationId, Guid.Parse(message.MessageId))
            {
                Source = message.Label
            };
        }

        private Task<bool> SendEncryptedAsync(ServiceBusCommandBase command)
        {
            return Task.Run(() =>
            {
                if (_encryptedClient != null)
                {
                    lock (_encryptedClient)
                    {
                        _encryptedClient.Send(CreateEncryptedMessage(command));
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
                        _plainClient.Send(CreatePlainCommand(command));
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
                _commands.Add(new CommandItem(command.CommandId, command.CommandName), new WeakReference<ServiceBusCommandBase>(command));
            }
        }

        private struct CommandItem : IEquatable<CommandItem>
        {
            public CommandItem(Guid commandId, string commandName)
            {
                _commandId = commandId;
                _commandName = commandName;
            }

            public bool Equals(CommandItem other)
            {
                return string.Equals(_commandName, other._commandName) && _commandId.Equals(other._commandId);
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
                    return (_commandName.GetHashCode() * 397) ^ _commandId.GetHashCode();
                }
            }

            private readonly string _commandName;
            private readonly Guid _commandId;
        }
    }
}