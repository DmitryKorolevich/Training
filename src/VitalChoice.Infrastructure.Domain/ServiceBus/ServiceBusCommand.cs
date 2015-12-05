using System;
using System.Threading;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public class ServiceBusCommand : ServiceBusCommandBase
    {
        public ServiceBusCommand(Guid sessionId, string commandName, string destination, Guid? commandId = null) : base(sessionId, commandName, destination, commandId)
        {
            ReadyEvent = new ManualResetEvent(false);
            RequestAcqureAction = (command, result) =>
            {
                var currentCommand = (ServiceBusCommand) command;
                currentCommand.Result = result;
                currentCommand.ReadyEvent.Set();
            };
        }

        public ServiceBusCommand(ServiceBusCommandBase initialCommand, object data) : base(initialCommand, data)
        {
            ReadyEvent = new ManualResetEvent(false);
            RequestAcqureAction = (command, result) =>
            {
                var currentCommand = (ServiceBusCommand)command;
                currentCommand.Result = result;
                currentCommand.ReadyEvent.Set();
            };
        }

        public ManualResetEvent ReadyEvent { get; }
        public object Result { get; set; }
    }
}