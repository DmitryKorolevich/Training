using System.Runtime.Serialization;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Dynamic.Masking;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class PaymentMethodDynamic : MappedObject
    {
        [DataMember]
        public AddressDynamic Address { get; set; }
    }
}