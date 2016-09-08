using System;
using System.Runtime.Serialization;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public delegate void CommandCompleteEventHandler(ServiceBusCommandBase command);

    [DataContract]
    public class ServiceBusCommandBase : IDisposable
    {
        public ServiceBusCommandBase(Guid sessionId, string commandName, string destination, string source, Guid? commandId = null,
            TimeSpan? ttl = null)
        {
            CommandName = commandName;
            SessionId = sessionId;
            CommandId = commandId ?? Guid.NewGuid();
            Destination = destination;
            Source = source;
            TimeToLeave = ttl ?? TimeSpan.FromSeconds(10);
            Data = new ServiceBusCommandData();
        }

        public ServiceBusCommandBase(ServiceBusCommandBase remoteCommand, object data)
        {
            CommandName = remoteCommand.CommandName;
            SessionId = remoteCommand.SessionId;
            CommandId = remoteCommand.CommandId;
            TimeToLeave = remoteCommand.TimeToLeave;
            Destination = remoteCommand.Source;
            Source = remoteCommand.Destination;
            Data = new ServiceBusCommandData(data);
        }

        public ServiceBusCommandBase(ServiceBusCommandBase remoteCommand, string error)
        {
            CommandName = remoteCommand.CommandName;
            SessionId = remoteCommand.SessionId;
            CommandId = remoteCommand.CommandId;
            TimeToLeave = remoteCommand.TimeToLeave;
            Destination = remoteCommand.Source;
            Source = remoteCommand.Destination;
            Data = new ServiceBusCommandData(error);
        }

        [DataMember]
        public Guid SessionId { get; set; }

        [DataMember]
        public Guid CommandId { get; set; }

        [DataMember]
        public TimeSpan TimeToLeave { get; set; }

        [DataMember]
        public string CommandName { get; set; }

        [DataMember]
        public string Destination { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public ServiceBusCommandData Data { get; set; }

        public Action<ServiceBusCommandBase, ServiceBusCommandData> RequestAcqureAction { get; set; }
        public CommandCompleteEventHandler OnComplete;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            OnComplete?.Invoke(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~ServiceBusCommandBase()
        {
            Dispose(false);
        }
    }
}