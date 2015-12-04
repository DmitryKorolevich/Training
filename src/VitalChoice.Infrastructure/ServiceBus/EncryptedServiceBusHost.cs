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
        private readonly ManualResetEvent _readyToDisposeEncrypted;
        private readonly ManualResetEvent _disposeEvent;
        private readonly ManualResetEvent _waitThreadStart = new ManualResetEvent(false);
        private readonly Dictionary<CommandItem, ServiceBusCommandBase> _commands;

        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;
        private readonly string _ignoreLocalLabel = Guid.NewGuid().ToString();

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger, IObjectEncryptionHost encryptionHost)
        {
            EncryptionHost = encryptionHost;
            _readyToDisposePlain = new ManualResetEvent(true);
            _readyToDisposeEncrypted = new ManualResetEvent(true);
            Logger = logger;
            _disposeEvent = new ManualResetEvent(false);
#if NET451 || DNX451
            try
            {
                _encryptedClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                    appOptions.Value.ExportService.EncryptedQueueName);
                _plainClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                    appOptions.Value.ExportService.PlainQueueName);
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message, e);
            }
#endif
            _commands = new Dictionary<CommandItem, ServiceBusCommandBase>();

            var thread = new Thread(ReceiveThread);
            thread.Start();
            _waitThreadStart.WaitOne();
        }

        protected void SendPlainCommand(ServiceBusCommandBase command)
        {
#if NET451 || DNX451
            _plainClient.Send(
                new BrokeredMessage(EncryptionHost.RsaSignWithConvert(command.Data))
                {
                    ContentType = command.CommandName,
                    SessionId = command.SessionId.ToString(),
                    MessageId = command.CommandId.ToString(),
                    TimeToLive = command.TimeToLeave,
                    Label = _ignoreLocalLabel
                });
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        protected T ExecutePlainCommand<T>(ServiceBusCommand command)
        {
            command.OnComplete = CommandComplete;
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.CommandId, command.CommandName), command);
            }
#if NET451 || DNX451
            _plainClient.Send(
                new BrokeredMessage(EncryptionHost.RsaSignWithConvert(command.Data))
                {
                    ContentType = command.CommandName,
                    SessionId = command.SessionId.ToString(),
                    MessageId = command.CommandId.ToString(),
                    TimeToLive = command.TimeToLeave,
                    Label = _ignoreLocalLabel
                });
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
            //BUG: set maximum wait time of command result receive to 20 minutes
            if (!command.ReadyEvent.WaitOne(command.TimeToLeave))
            {
                throw new TimeoutException($"Command timeout. <{command.CommandName}>({command.CommandId})");
            }
#endif
            CommandComplete(command);
            return (T) command.Result;
        }

        public void SendCommand(ServiceBusCommandBase command)
        {
#if NET451 || DNX451
            _encryptedClient.Send(new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave,
                Label = _ignoreLocalLabel
            });
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public Task<T> ExecuteCommand<T>(ServiceBusCommand command)
        {
            command.OnComplete = CommandComplete;
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.CommandId, command.CommandName), command);
            }
#if NET451 || DNX451
            _encryptedClient.Send(new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave,
                Label = _ignoreLocalLabel
            });
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
            return Task.Run(() =>
            {
                //BUG: set maximum wait time of command result receive to 5 minutes
                if (!command.ReadyEvent.WaitOne(command.TimeToLeave))
                {
                    throw new TimeoutException($"Command timeout. <{command.CommandName}>({command.CommandId})");
                }
                CommandComplete(command);
                return (T) command.Result;
            });
        }

        public void ExecuteCommand(ServiceBusCommandBase command,
            Action<ServiceBusCommandBase, object> commandResultAction)
        {
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.CommandId, command.CommandName), command);
            }
            command.RequestAcqureAction = commandResultAction;
#if NET451 || DNX451
            _plainClient.Send(new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave,
                Label = _ignoreLocalLabel
            });
#endif
            Logger.LogInformation($"{command.CommandName} sent ({command.CommandId})");
        }

        public bool IsAuthenticatedClient(Guid sessionId)
        {
            return EncryptionHost.SessionExist(sessionId);
        }

        protected virtual bool ProcessEncryptedCommand(ServiceBusCommand command)
        {
            return false;
        }

        protected virtual bool ProcessPlainCommand(ServiceBusCommand command)
        {
            return false;
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

        private void ReceiveThread()
        {
#if NET451 || DNX451
            _plainClient.OnMessage(ProcessPlainMessage);
            _encryptedClient.OnMessage(ProcessEncryptedMessage);
#endif
            _waitThreadStart.Set();
            _waitThreadStart.Dispose();
            _disposeEvent.WaitOne();
        }

#if NET451 || DNX451
        private void ProcessEncryptedMessage(BrokeredMessage message)
        {
            try
            {
                if (message.Label == _ignoreLocalLabel)
                    return;
                _readyToDisposeEncrypted.Reset();
                ServiceBusCommandBase command;
                lock (_commands)
                {
                    _commands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType),
                        out command);
                }
                if (command != null)
                {
                    Logger.LogInformation($"{command.CommandName} intercepting ({command.CommandId})");
                    var body = EncryptionHost.AesDecrypt<object>(message.GetBody<byte[]>(), command.SessionId);
                    command.RequestAcqureAction?.Invoke(command, body);
                    message.Complete();
                }
                else
                {
                    var incomingCommand = new ServiceBusCommand(Guid.Parse(message.MessageId), message.ContentType,
                        Guid.Parse(message.MessageId));
                    Logger.LogInformation($"{incomingCommand.CommandName} received ({incomingCommand.CommandId})");

                    var body = EncryptionHost.AesDecrypt<object>(message.GetBody<byte[]>(), incomingCommand.SessionId);
                    incomingCommand.RequestAcqureAction?.Invoke(incomingCommand, body);
                    if (ProcessEncryptedCommand(incomingCommand))
                    {
                        Logger.LogInformation($"{incomingCommand.CommandName} processing success ({incomingCommand.CommandId})");
                        message.Complete();
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
                _readyToDisposeEncrypted.Set();
            }
        }

        private void ProcessPlainMessage(BrokeredMessage message)
        {
            try
            {
                if (message.Label == _ignoreLocalLabel)
                    return;
                _readyToDisposePlain.Reset();
                ServiceBusCommandBase command;
                lock (_commands)
                {
                    _commands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType),
                        out command);
                }
                object body;
                if (EncryptionHost.RsaCheckSignWithConvert(message.GetBody<PlainCommandData>(), out body))
                {
                    Logger.LogInformation($"incoming {message.ContentType} identity validate sucess.");
                    if (command != null)
                    {
                        Logger.LogInformation($"{command.CommandName} intercepting ({command.CommandId})");
                        command.RequestAcqureAction?.Invoke(command, body);
                        message.Complete();
                    }
                    else
                    {
                        var incomingCommand = new ServiceBusCommand(Guid.Parse(message.MessageId), message.ContentType,
                            Guid.Parse(message.MessageId));
                        Logger.LogInformation($"{incomingCommand.CommandName} received ({incomingCommand.CommandId})");
                        incomingCommand.RequestAcqureAction?.Invoke(incomingCommand, body);
                        if (ProcessPlainCommand(incomingCommand))
                        {
                            message.Complete();
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
                _readyToDisposePlain.Set();
            }
        }
#endif

        public virtual void Dispose()
        {
            _disposeEvent.Set();
            WaitHandle.WaitAll(new WaitHandle[] {_readyToDisposeEncrypted, _readyToDisposePlain},
                TimeSpan.FromSeconds(20));
#if NET451 || DNX451
            _plainClient?.Close();
            _encryptedClient?.Close();
#endif
            EncryptionHost?.Dispose();
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