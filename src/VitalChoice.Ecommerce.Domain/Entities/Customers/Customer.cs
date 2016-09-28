using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class Customer : DynamicDataEntity<CustomerOptionValue, CustomerOptionType>
	{
	    public Customer()
	    {
			PaymentMethods = new List<CustomerToPaymentMethod>();
			OrderNotes = new List<CustomerToOrderNote>();
			ShippingAddresses = new List<CustomerToShippingAddress>();
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

        public int IdProfileAddress { get; set; }

        public Address ProfileAddress { get; set; }

        public ICollection<CustomerToPaymentMethod> PaymentMethods { get; set; }

	    public ICollection<CustomerToOrderNote> OrderNotes { get; set; }

	    public ICollection<CustomerToShippingAddress> ShippingAddresses { get; set; }

	    public ICollection<CustomerNote> CustomerNotes { get; set; }

	    public ICollection<CustomerPaymentMethod> CustomerPaymentMethods { get; set; }

	    public ICollection<CustomerFile> Files { get; set; }
	}
}
