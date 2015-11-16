using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public class CustomerPaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }

        public int IdCustomer { get; set; }
    }
}