using System;
using System.Threading;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public class ServiceBusCommand : ServiceBusCommandBase
    {
        public ServiceBusCommand(Guid sessionId, string commandName, Guid? commandId = null) : base(sessionId, commandName, commandId)
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
            Data = data;
        }

        public ManualResetEvent ReadyEvent { get; }
        public object Result { get; set; }
    }
}