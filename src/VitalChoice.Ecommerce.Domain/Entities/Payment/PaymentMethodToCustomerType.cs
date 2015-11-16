using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Ecommerce.Domain.Entities.Payment
{
    public class PaymentMethodToCustomerType:Entity
    {
		public PaymentMethod PaymentMethod { get; set; }

		public CustomerTypeEntity CustomerType { get; set; }

		public int IdPaymentMethod { get; set; }

		public int IdCustomerType { get; set; }
    }
}
