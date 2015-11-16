using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ExtendedOrderNote: OrderNote
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
