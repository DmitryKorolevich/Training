using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    public class AddressDynamic : MappedObject
    {
        [DataMember]
        public int? IdCountry { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public int? IdState { get; set; }
    }
}