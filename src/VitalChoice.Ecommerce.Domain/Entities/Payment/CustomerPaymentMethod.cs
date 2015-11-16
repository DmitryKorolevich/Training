using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Payment
{
    public class CustomerPaymentMethod: DynamicDataEntity<CustomerPaymentMethodOptionValue, CustomerPaymentMethodOptionType>
    {
		public int IdCustomer { get; set; }

	    public PaymentMethod PaymentMethod { get; set; }

		public Address BillingAddress { get; set; }

		public int? IdAddress { get; set; }
	}
}