using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderFilter : FilterBase
    {
        public string Id { get; set; }

	    public int? IdCustomer { get; set; }

	    public OrderType? OrderType { get; set; }
    }
}