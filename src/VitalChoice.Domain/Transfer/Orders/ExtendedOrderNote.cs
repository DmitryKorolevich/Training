using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Transfer.Orders
{
    public class ExtendedOrderNote: OrderNote
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
