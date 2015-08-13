using System;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerFile : Entity
	{
	    public int IdCustomer { get; set; }

	    public DateTime UploadDate { get; set; }

	    public string FileName { get; set; }

	    public string Description { get; set; }
	}
}
