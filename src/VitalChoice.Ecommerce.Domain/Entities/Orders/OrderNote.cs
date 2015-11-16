using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderNote: Entity
    {
	    public OrderNote()
	    {
			CustomerTypes = new List<OrderNoteToCustomerType>();
	    }

	    public string Title { get; set; }

	    public string Description { get; set; }

	    public RecordStatusCode StatusCode { get; set; }

	    public DateTime DateCreated { get; set; }

		public DateTime DateEdited { get; set; }

		public int? IdEditedBy { get; set; }

		public User EditedBy { get; set; }

	    public ICollection<OrderNoteToCustomerType> CustomerTypes { get; set; }

	    public ICollection<CustomerToOrderNote> Customers { get; set; }
    }
}
