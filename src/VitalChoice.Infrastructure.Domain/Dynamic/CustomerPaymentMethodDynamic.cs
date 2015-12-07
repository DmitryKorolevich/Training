using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    [DataContract]
    public class CustomerPaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }

        [DataMember]
        public int IdCustomer { get; set; }
    }
}