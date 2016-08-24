using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VitalChoice.Core.Infrastructure.Models
{
    [DataContract]
    public class ClientAuthorization
    {
        [DataMember]
        public byte[] WholeHash { get; set; }

        [DataMember]
        public Guid AuthToken { get; set; }
    }
}