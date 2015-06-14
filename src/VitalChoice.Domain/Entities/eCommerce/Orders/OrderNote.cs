using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public class OrderNote: Entity
    {
	    public string Title { get; set; }

	    public string Description { get; set; }

	    public RecordStatusCode StatusCode { get; set; }

	    public DateTime DateCreated { get; set; }

		public DateTime DateEdited { get; set; }

		public int? IdEditedBy { get; set; }

		public User EditedBy { get; set; }

	    public ICollection<OrderNoteToCustomerType> CustomerTypes { get; set; }
    }
}
