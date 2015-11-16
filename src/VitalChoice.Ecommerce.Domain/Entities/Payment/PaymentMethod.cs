using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Payment
{
    public class PaymentMethod: Entity
    {
	    public PaymentMethod()
	    {
			CustomerTypes = new List<PaymentMethodToCustomerType>();
        }

	    public string Name { get; set; }

		public RecordStatusCode StatusCode { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime DateEdited { get; set; }

		public int? IdEditedBy { get; set; }

		public User EditedBy { get; set; }

		public ICollection<PaymentMethodToCustomerType> CustomerTypes { get; set; }

	    public int Order { get; set; }

	    public ICollection<CustomerToPaymentMethod> Customers { get; set; }
    }
}
