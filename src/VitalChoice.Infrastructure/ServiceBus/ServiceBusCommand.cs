using System;
using System.Threading;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public class ServiceBusCommand : ServiceBusCommandBase
    {
        public ServiceBusCommand(Guid sessionId, string commandName) : base(sessionId, commandName)
        {
            ReadyEvent = new ManualResetEvent(false);
            RequestAcqureAction = (command, result) =>
            {
                var currentCommand = (ServiceBusCommand) command;
                currentCommand.Result = result;
                currentCommand.ReadyEvent.Set();
            };
        }

        public ManualResetEvent ReadyEvent { get; }
        public object Result { get; set; }
    }
}