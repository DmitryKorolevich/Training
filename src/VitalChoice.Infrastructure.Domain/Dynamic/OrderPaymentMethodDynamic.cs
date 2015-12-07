using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    public sealed class OrderPaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }

        [DataMember]
        public int IdOrder { get; set; }
    }
}
