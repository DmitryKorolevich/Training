using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderFilter : OrderFilterBase
	{
        public OrderType? OrderType { get; set; }
    }
}