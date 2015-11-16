using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerTypeEntity: Entity
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
