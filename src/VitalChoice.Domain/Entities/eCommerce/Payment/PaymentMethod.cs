using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
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

		public virtual ICollection<PaymentMethodToCustomerType> CustomerTypes { get; set; }

	    public int Order { get; set; }
    }
}
