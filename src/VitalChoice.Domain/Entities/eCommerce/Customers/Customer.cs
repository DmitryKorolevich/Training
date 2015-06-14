using System;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class Customer : Entity
	{
	    public User User { get; set; }

	    public DateTime DateCreated { get; set; }

	    public DateTime DateEdited { get; set; }

	    public int? IdEditedBy { get; set; }

	    public User EditedBy { get; set; }

	    public int IdCustomerType { get; set; }

	    public CustomerType CustomerType { get; set; }
	}
}
