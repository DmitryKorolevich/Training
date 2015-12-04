using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class OrderPaymentMethodDynamic : MappedObject
    {
        [DataMember]
        public AddressDynamic Address { get; set; }

        [DataMember]
        public int IdOrder { get; set; }
    }
}
