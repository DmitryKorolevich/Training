using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class Customer : DynamicDataEntity<CustomerOptionValue, CustomerOptionType>
	{
	    public Customer()
	    {
			PaymentMethods = new List<CustomerToPaymentMethod>();
			OrderNotes = new List<CustomerToOrderNote>();
			Addresses = new List<Address>();
			CustomerNotes = new List<CustomerNote>();
			CustomerPaymentMethods = new List<CustomerPaymentMethod>();
        }

	    public User User { get; set; }

	    public string Email { get; set; }

	    public Guid PublicId { get; set; }

	    public CustomerTypeEntity CustomerType { get; set; }

	    public int IdDefaultPaymentMethod { get; set; }

	    public PaymentMethod DefaultPaymentMethod { get; set; }

        public int? IdAffiliate { get; set; }

        public ICollection<CustomerToPaymentMethod> PaymentMethods { get; set; }

	    public ICollection<CustomerToOrderNote> OrderNotes { get; set; }

	    public ICollection<Address> Addresses { get; set; }

	    public ICollection<CustomerNote> CustomerNotes { get; set; }

	    public ICollection<CustomerPaymentMethod> CustomerPaymentMethods { get; set; }

	    public ICollection<CustomerFile> Files { get; set; }
	}
}
