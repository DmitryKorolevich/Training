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
        public ServiceBusCommandData(string error) : this()
        {
            Error = error;
        }

        public ServiceBusCommandData()
        {
            Random rnd = new Random();
            byte[] elements = new byte[8];
            rnd.NextBytes(elements);
            Data = elements;
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