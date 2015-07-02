using System.Collections.Generic;
using System.Linq;
using NuGet;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerDynamic : MappedObject
    {
		public User User { get; set; }

		public string Email { get; set; }

		public CustomerType CustomerType { get; set; }

		public int IdDefaultPaymentMethod { get; set; }

		public ICollection<int> ApprovedPaymentMethods { get; set; }

		public ICollection<int> OrderNotes { get; set; }

		public ICollection<AddressDynamic> Addresses { get; set; }

		public ICollection<CustomerNote> CustomerNotes { get; set; }
    }
}
