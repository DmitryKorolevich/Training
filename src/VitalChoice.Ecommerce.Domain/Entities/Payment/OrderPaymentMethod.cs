using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Payment
{
    public class OrderPaymentMethod : DynamicDataEntity<OrderPaymentMethodOptionValue, CustomerPaymentMethodOptionType>
    {
        public int? IdAddress { get; set; }

        public OrderAddress BillingAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}