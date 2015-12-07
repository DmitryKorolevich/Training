using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Payment
{
    public class OrderPaymentMethodOptionValue : OptionValue<CustomerPaymentMethodOptionType>
    {
        public int IdOrderPaymentMethod {
            get;
            set; }
    }
}