using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
#if NET451 || DNX451
using Microsoft.ServiceBus.Messaging;
#endif
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public class EncryptedServiceBusClientHost : IDisposable, IEncryptedServiceBusClientHost
    {
#if NET451 || DNX451
        private readonly QueueClient _sendClient;
        private readonly QueueClient _receiveClient;
#endif
        private readonly ManualResetEvent _disposeEvent;
        private readonly Dictionary<CommandItem, ServiceBusCommandBase> _exportCommands;
        private readonly ObjectEncryptionHost _encryptionHost;
        public EncryptedServiceBusClientHost(IOptions<AppOptions> appOptions)
        {
            _disposeEvent = new ManualResetEvent(false);
#if NET451 || DNX451
            _receiveClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                appOptions.Value.ExportService.ReceiveQueueName);
            _sendClient = QueueClient.CreateFromConnectionString(appOptions.Value.ExportService.ConnectionString,
                appOptions.Value.ExportService.SendQueueName);
#endif
            _exportCommands = new Dictionary<CommandItem, ServiceBusCommandBase>();

            var thread = new Thread(ReceiveThread);
            thread.Start();

            _encryptionHost = new ObjectEncryptionHost(false);
        }

        private T ExecutePlainCommand<T>(ServiceBusCommand command)
        {
            command.OnComplete = CommandComplete;
            lock (_exportCommands)
            {
                _exportCommands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
#if NET451 || DNX451
            _sendClient.Send(new BrokeredMessage(command.Data)
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = TimeSpan.FromMinutes(20)
            });

            //BUG: set maximum wait time of command result receive to 20 minutes
            if (!command.ReadyEvent.WaitOne(new TimeSpan(0, 20, 0)))
            {
                throw new ApiException($"Command timeout. <{command.CommandName}>");
            }
#endif
            CommandComplete(command);
            return (T)command.Result;
        }

        public Task<T> ExecuteCommand<T>(ServiceBusCommand command)
        {
            command.OnComplete = CommandComplete;
            lock (_exportCommands)
            {
                _exportCommands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
#if NET451 || DNX451
            _sendClient.Send(new BrokeredMessage(_encryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = TimeSpan.FromMinutes(5)
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
                return (T)command.Result;
            });
        }

        public void RunCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> requestAcqureAction)
        {
            lock (_exportCommands)
            {
                _exportCommands.Add(new CommandItem(command.SessionId, command.CommandName), command);
            }
            command.RequestAcqureAction = requestAcqureAction;
#if NET451 || DNX451
            _sendClient.Send(new BrokeredMessage(_encryptionHost.AesEncrypt(command.Data, command.SessionId))
            {
                ContentType = command.CommandName,
                SessionId = command.SessionId.ToString(),
                MessageId = command.CommandId.ToString(),
                TimeToLive = TimeSpan.FromMinutes(20)
            });
#endif
        }

        public bool IsAuthenticatedClient(Guid sessionId)
        {
            return _encryptionHost.SessionExist(sessionId);
        }

        public bool AuthenticateClient(Guid sessionId)
        {
            var keys = _encryptionHost.CreateSession(sessionId);
            if (keys == null)
            {
                if (ExecutePlainCommand<bool>(new ServiceBusCommand(sessionId, OrderExportServiceConstants.CheckSessionKey)))
                    return true;
                keys = _encryptionHost.GetSessionWithReset(sessionId);
            }
            return ExecutePlainCommand<bool>(new ServiceBusCommand(sessionId, OrderExportServiceConstants.SetSessionKey)
            {
                Data = _encryptionHost.RsaEncrypt(keys)
            });
        }

        public void RemoveClient(Guid sessionId)
        {
            _encryptionHost.RemoveSession(sessionId);
        }

        private void CommandComplete(ServiceBusCommandBase command)
        {
            lock (_exportCommands)
            {
                var commandItem = new CommandItem(command.CommandId, command.CommandName);
                if (_exportCommands.ContainsKey(commandItem))
                {
                    _exportCommands.Remove(commandItem);
                }
            }
        }

        private void ReceiveThread()
        {
#if NET451 || DNX451
            _sendClient.OnMessage(message =>
            {
                ServiceBusCommandBase commandBase;
                if (message.ContentType == OrderExportServiceConstants.SessionExpired)
                {
                    var session = message.GetBody<Guid>();
                    if (_encryptionHost.RemoveSession(session))
                    {
                        message.Complete();
                    }
                    return;
                }
                lock (_exportCommands)
                {
                    _exportCommands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType), out commandBase);
                }
                if (commandBase != null)
                {
                    object body;
                    switch (commandBase.CommandName)
                    {
                        case OrderExportServiceConstants.SetSessionKey:
                            body = message.GetBody<bool>();
                            break;
                        default:
                            body = null;
                            break;
                    }
                    commandBase.RequestAcqureAction(commandBase, body);
                    message.Complete();
                }
            });

            _receiveClient.OnMessage(message =>
            {
                ServiceBusCommandBase commandBase;
                lock (_exportCommands)
                {
                    _exportCommands.TryGetValue(new CommandItem(Guid.Parse(message.MessageId), message.ContentType), out commandBase);
                }
                if (commandBase != null)
                {
                    var body = message.GetBody<byte[]>();
                    commandBase.RequestAcqureAction(commandBase, _encryptionHost.AesDecrypt<object>(body, commandBase.SessionId));
                    message.Complete();
                }
            });
#endif
            _disposeEvent.WaitOne();
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

        public void Dispose()
        {
            _disposeEvent.Set();
#if NET451 || DNX451
            _sendClient?.Close();
            _receiveClient?.Close();
#endif
            _encryptionHost?.Dispose();
        }
    }
}