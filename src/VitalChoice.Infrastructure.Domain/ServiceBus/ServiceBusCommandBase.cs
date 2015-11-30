using System;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public delegate void CommandCompleteEventHandler(ServiceBusCommandBase command);

    public class ServiceBusCommandBase : IDisposable
    {
        public ServiceBusCommandBase(Guid sessionId, string commandName, Guid? commandId = null, TimeSpan? ttl = null)
        {
            CommandName = commandName;
            SessionId = sessionId;
            CommandId = commandId ?? new Guid();
            TimeToLeave = ttl ?? TimeSpan.FromMinutes(20);
        }

        public ServiceBusCommandBase(ServiceBusCommandBase initialCommand, object data)
        {
            CommandName = initialCommand.CommandName;
            SessionId = initialCommand.SessionId;
            CommandId = initialCommand.CommandId;
            TimeToLeave = initialCommand.TimeToLeave;
            Data = data;
        }

        public Guid SessionId { get; }
        public Guid CommandId { get; }
        public TimeSpan TimeToLeave { get; }
        public string CommandName { get; }
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