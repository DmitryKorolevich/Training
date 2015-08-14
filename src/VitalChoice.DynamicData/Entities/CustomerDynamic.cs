using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerDynamic : MappedObject
    {
	    public CustomerDynamic()
	    {
			User = new User();
			ApprovedPaymentMethods = new List<int>();
			OrderNotes = new List<int>();
			Addresses = new List<AddressDynamic>();
			CustomerNotes = new List<CustomerNoteDynamic>();
            CustomerPaymentMethods = new List<CustomerPaymentMethodDynamic>();
			Files = new List<CustomerFile>();
        }

	    public User User { get; set; }

		public string Email { get; set; }

		public int IdDefaultPaymentMethod { get; set; }

		public Guid PublicId { get; set; }

		public string EditedBy { get; set; }

	    public ICollection<int> ApprovedPaymentMethods { get; set; }

		public ICollection<int> OrderNotes { get; set; }

		public ICollection<AddressDynamic> Addresses { get; set; }

		public ICollection<CustomerNoteDynamic> CustomerNotes { get; set; }

        public ICollection<CustomerPaymentMethodDynamic> CustomerPaymentMethods { get; set; }

	    public ICollection<CustomerFile> Files { get; set; }
    }
}
