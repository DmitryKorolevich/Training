using System;

namespace VitalChoice.Infrastructure.ServiceBus
{
    internal delegate void CommandCompleteEventHandler(ServiceBusCommandBase command);

    public class ServiceBusCommandBase : IDisposable
    {
        protected ServiceBusCommandBase(Guid sessionId, string commandName)
        {
            CommandName = commandName;
            SessionId = sessionId;
        }

        public Guid SessionId { get; }
        public Guid CommandId { get; } = new Guid();
        public string CommandName { get; }
        public object Data { get; set; }

        internal Action<ServiceBusCommandBase, object> RequestAcqureAction { get; set; }
        internal CommandCompleteEventHandler OnComplete;

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