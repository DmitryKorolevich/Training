using VitalChoice.Domain.Entities.eCommerce.Payment;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerToPaymentMethod : Entity
	{
		public int IdCustomer { get; set; }

	    public Customer Customer { get; set; }

	    public int IdPaymentMethod { get; set; }

	    public PaymentMethod PaymentMethod { get; set; }
	}
}
