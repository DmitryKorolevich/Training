using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    [DataContract]
    public class ServiceBusCommandData
    {
        public ServiceBusCommandData(string error)
        {
            Error = error;
        }

        public ServiceBusCommandData()
        {

        }

        public ServiceBusCommandData(object data)
        {
            Data = data;
        }

        [DataMember]
        public object Data { get; set; }

        [DataMember]
        public string Error { get; set; }
    }
}