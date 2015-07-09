using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Transfer.Customers
{
    public class ExtendedVCustomer: VCustomer
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
