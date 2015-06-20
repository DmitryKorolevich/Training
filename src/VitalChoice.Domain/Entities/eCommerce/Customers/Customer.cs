using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class Customer : DynamicDataEntity<CustomerOptionValue, CustomerOptionType>
	{
	    public User User { get; set; }

	    public string FirstName { get; set; }

	    public string LastName { get; set; }

	    public string Email { get; set; }

	    public int IdCustomerType { get; set; }

	    public CustomerTypeEntity CustomerType { get; set; }

	    public int IdDefaultPaymentMethod { get; set; }

	    public PaymentMethod DefaultPaymentMethod { get; set; }

	    public ICollection<CustomerToPaymentMethod> PaymentMethods { get; set; }

	    public ICollection<CustomerToOrderNote> OrderNotes { get; set; }

	    public ICollection<Address> Addresses { get; set; }

	    public ICollection<CustomerNote> CustomerNotes { get; set; }
	}
}
