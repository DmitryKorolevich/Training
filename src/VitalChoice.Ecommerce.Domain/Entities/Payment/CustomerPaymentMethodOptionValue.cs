using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Payment
{
    public class CustomerPaymentMethodOptionValue : OptionValue<CustomerPaymentMethodOptionType>
    {
        public int IdCustomerPaymentMethod { get; set; }
    }
}