using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class ExtendedVCustomer: VCustomer
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
