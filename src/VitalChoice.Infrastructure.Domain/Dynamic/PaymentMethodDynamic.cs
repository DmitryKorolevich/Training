using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    public abstract class PaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }
    }
}