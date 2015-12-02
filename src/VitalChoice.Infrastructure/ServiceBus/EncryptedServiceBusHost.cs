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
        private readonly ManualResetEvent _disposeEvent;
        private readonly Dictionary<CommandItem, ServiceBusCommandBase> _commands;

        protected abstract ObjectEncryptionHost EncryptionHost { get; }
        protected readonly ILogger Logger;

        protected EncryptedServiceBusHost(IOptions<AppOptions> appOptions, ILogger logger)
        {
            Logger = logger;
            _disposeEvent = new ManualResetEvent(false);
#if NET451 || DNX451
            _encryptedClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                appOptions.Value.ExportService.EncryptedQueueName);
            _plainClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                appOptions.Value.ExportService.PlainQueueName);
#endif
            _commands = new Dictionary<CommandItem, ServiceBusCommandBase>();

            var thread = new Thread(ReceiveThread);
            thread.Start();
        }

        protected void SendPlainCommand(ServiceBusCommandBase command)
        {
            command.OnComplete = CommandComplete;
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
#if NET451 || DNX451
            _plainClient.Send(
                new BrokeredMessage(new PlainCommandData
                {
                    Data = EncryptionHost.RsaEncrypt(command.Data),
                    Certificate = EncryptionHost.CurrentCert
                })
                {
                    ContentType = command.CommandName,
                    SessionId = command.SessionId.ToString(),
                    MessageId = command.CommandId.ToString(),
                    TimeToLive = command.TimeToLeave
                });
#endif
            CommandComplete(command);
        }

        protected T ExecutePlainCommand<T>(ServiceBusCommand command)
        {
            command.OnComplete = CommandComplete;
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
#if NET451 || DNX451
            _plainClient.Send(
                new BrokeredMessage(new PlainCommandData
                {
                    Data = EncryptionHost.RsaEncrypt(command.Data),
                    Certificate = EncryptionHost.CurrentCert
                })
                {
                    ContentType = command.CommandName,
                    SessionId = command.SessionId.ToString(),
                    MessageId = command.CommandId.ToString(),
                    TimeToLive = command.TimeToLeave
                });

            //BUG: set maximum wait time of command result receive to 20 minutes
            if (!command.ReadyEvent.WaitOne(new TimeSpan(0, 20, 0)))
            {
                throw new ApiException($"Command timeout. <{command.CommandName}>");
            }
#endif
            CommandComplete(command);
            return EncryptionHost.RsaDecrypt<T>(command.Result as byte[]);
        }

        public void SendCommand(ServiceBusCommandBase command)
        {
            command.OnComplete = CommandComplete;
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
#if NET451 || DNX451
            _encryptedClient.Send(new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave
            });
#endif
            CommandComplete(command);
        }

        public Task<T> ExecuteCommand<T>(ServiceBusCommand command)
        {
            command.OnComplete = CommandComplete;
            lock (_commands)
            {
                _commands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
#if NET451 || DNX451
            _encryptedClient.Send(new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave
            });
#endif
            return Task.Run(() =>
            {
                //BUG: set maximum wait time of command result receive to 5 minutes
                if (!command.ReadyEvent.WaitOne(new TimeSpan(0, 5, 0)))
                {
                    throw new ApiException($"Command timeout. <{command.CommandName}>");
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
                _commands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
            command.RequestAcqureAction = commandResultAction;
#if NET451 || DNX451
            _plainClient.Send(new BrokeredMessage(EncryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = command.TimeToLeave
            });
#endif
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
            _disposeEvent.WaitOne();
        }

#if NET451 || DNX451
        private void ProcessEncryptedMessage(BrokeredMessage message)
        {
            try
            {
                ServiceBusCommandBase command;
                lock (_commands)
                {
                    _commands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType),
                        out command);
                }
                if (command != null)
                {
                    var body = EncryptionHost.AesDecrypt<object>(message.GetBody<byte[]>(), command.SessionId);
                    command.RequestAcqureAction?.Invoke(command, body);
                    message.Complete();
                }
                else
                {
                    var incomingCommand = new ServiceBusCommand(Guid.Parse(message.MessageId), message.ContentType,
                        Guid.Parse(message.MessageId));

                    var body = EncryptionHost.AesDecrypt<object>(message.GetBody<byte[]>(), incomingCommand.SessionId);
                    incomingCommand.RequestAcqureAction?.Invoke(incomingCommand, body);
                    if (ProcessEncryptedCommand(incomingCommand))
                    {
                        message.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("<ProcessEncryptedMessage> Error while processing incoming message", e);
                throw;
            }
        }

        private void ProcessPlainMessage(BrokeredMessage message)
        {
            try
            {
                ServiceBusCommandBase command;
                lock (_commands)
                {
                    _commands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType),
                        out command);
                }
                X509Certificate2 serverCert, clientCert;
                var commandData = message.GetBody<PlainCommandData>();
                if (EncryptionHost.Server)
                {
                    serverCert = commandData.Certificate;
                    clientCert = EncryptionHost.ClientCert;
                }
                else
                {
                    serverCert = EncryptionHost.RootCert;
                    clientCert = commandData.Certificate;
                }
                if (EncryptionHost.ValidateClientCertificate(serverCert, clientCert))
                {
                    var rsa = (RSACryptoServiceProvider) commandData.Certificate.PublicKey.Key;
                    var body = EncryptionHost.RsaDecrypt<object>(commandData.Data, rsa);
                    if (command != null)
                    {
                        command.RequestAcqureAction?.Invoke(command, body);
                        message.Complete();
                    }
                    else
                    {
                        var incomingCommand = new ServiceBusCommand(Guid.Parse(message.MessageId), message.ContentType,
                            Guid.Parse(message.MessageId));
                        incomingCommand.RequestAcqureAction?.Invoke(incomingCommand, body);
                        if (ProcessPlainCommand(incomingCommand))
                        {
                            message.Complete();
                        }
                    }
                }
                else
                {
                    throw new AccessDeniedException("Invalid client certificate");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("<ProcessPlainMessage> Error while processing incoming message", e);
                throw;
            }
        }
#endif

        public virtual void Dispose()
        {
            _disposeEvent.Set();
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