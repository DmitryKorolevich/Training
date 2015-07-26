using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class CustomerPaymentMethodOptionValue : OptionValue<CustomerPaymentMethodOptionType>
    {
        public int IdCustomerPaymentMethod { get; set; }
    }
}