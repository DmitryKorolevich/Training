using System;
using System.Runtime.Serialization;
using System.Threading;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    [DataContract]
    public class ServiceBusCommandWithResult : ServiceBusCommandBase
    {
        public ServiceBusCommandWithResult(Guid sessionId, string commandName, string destination, string source, Guid? commandId = null) : base(sessionId, commandName, destination, source, commandId)
        {
            ReadyEvent = new ManualResetEvent(false);
            RequestAcqureAction = (command, result) =>
            {
                var currentCommand = (ServiceBusCommandWithResult) command;
                currentCommand.Result = result;
                currentCommand.ReadyEvent.Set();
            };
        }

        public ServiceBusCommandWithResult(ServiceBusCommandBase remoteCommand, object data) : base(remoteCommand, data)
        {
            ReadyEvent = new ManualResetEvent(false);
            RequestAcqureAction = (command, result) =>
            {
                var currentCommand = (ServiceBusCommandWithResult)command;
                currentCommand.Result = result;
                currentCommand.ReadyEvent.Set();
            };
        }

        public ManualResetEvent ReadyEvent { get; }
        public object Result { get; set; }
    }
}