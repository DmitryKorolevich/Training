using System;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public delegate void CommandCompleteEventHandler(ServiceBusCommandBase command);

    public class ServiceBusCommandBase : IDisposable
    {
        public ServiceBusCommandBase(Guid sessionId, string commandName, string destination, Guid? commandId = null, TimeSpan? ttl = null)
        {
            CommandName = commandName;
            SessionId = sessionId;
            CommandId = commandId ?? Guid.NewGuid();
            Destination = destination;
            TimeToLeave = ttl ?? TimeSpan.FromSeconds(30);
        }

        public ServiceBusCommandBase(ServiceBusCommandBase initialCommand, object data)
        {
            CommandName = initialCommand.CommandName;
            SessionId = initialCommand.SessionId;
            CommandId = initialCommand.CommandId;
            TimeToLeave = initialCommand.TimeToLeave;
            Destination = initialCommand.Source;
            Source = initialCommand.Destination;
            Data = data;
        }

        public Guid SessionId { get; }
        public Guid CommandId { get; }
        public TimeSpan TimeToLeave { get; }
        public string CommandName { get; }
        public string Destination { get; set; }
        public string Source { get; set; }
        public object Data { get; set; }

        public Action<ServiceBusCommandBase, object> RequestAcqureAction { get; set; }
        public CommandCompleteEventHandler OnComplete;

        protected void Dispose(bool disposing)
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