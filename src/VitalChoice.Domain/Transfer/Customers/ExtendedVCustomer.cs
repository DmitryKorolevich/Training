using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Helpers.Export;

namespace VitalChoice.Domain.Transfer.Customers
{
    public class ExtendedVCustomer: VCustomer, IExportable
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
