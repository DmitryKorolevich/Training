using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class CustomerPaymentMethod: DynamicDataEntity<CustomerPaymentMethodOptionValue, CustomerPaymentMethodOptionType>
	{
		public int IdCustomer { get; set; }

	    public PaymentMethod PaymentMethod { get; set; }

		public Address BillingAddress { get; set; }

		public int IdAddress { get; set; }
	}
}
