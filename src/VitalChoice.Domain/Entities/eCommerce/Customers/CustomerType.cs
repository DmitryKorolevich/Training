using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerType: Entity
    {
		public string Name { get; set; }

		public RecordStatusCode StatusCode { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime DateEdited { get; set; }

		public int? IdEditedBy { get; set; }

		public User EditedBy { get; set; }

	    public int Order { get; set; }

	    public ICollection<Customer> Customers { get; set; }

		public ICollection<PaymentMethodToCustomerType> PaymentMethods { get; set; }

		public ICollection<OrderNoteToCustomerType> OrderNotes { get; set; }
    }
}
