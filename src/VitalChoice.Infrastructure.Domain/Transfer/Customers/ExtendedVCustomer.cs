using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Helpers.Export;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class ExtendedVCustomer: VCustomer, IExportable
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
