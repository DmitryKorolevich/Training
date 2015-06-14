using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class PaymentMethodToCustomerType
    {
		public PaymentMethod PaymentMethod { get; set; }

		public CustomerType CustomerType { get; set; }

		public int IdPaymentMethod { get; set; }

		public int IdCustomerType { get; set; }
    }
}
