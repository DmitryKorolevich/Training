using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class OrderPaymentMethodDynamic : MappedObject
    {
        public AddressDynamic Address { get; set; }

        public int IdOrder { get; set; }
    }
}
