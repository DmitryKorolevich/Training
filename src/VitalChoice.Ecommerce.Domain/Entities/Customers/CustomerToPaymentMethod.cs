using VitalChoice.Ecommerce.Domain.Entities.Payment;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerToPaymentMethod : Entity
	{
		public int IdCustomer { get; set; }

	    public Customer Customer { get; set; }

	    public int IdPaymentMethod { get; set; }

	    public PaymentMethod PaymentMethod { get; set; }
	}
}
