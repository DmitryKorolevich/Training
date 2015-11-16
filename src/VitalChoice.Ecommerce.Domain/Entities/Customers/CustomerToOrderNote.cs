using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerToOrderNote : Entity
	{
		public int IdCustomer { get; set; }

		public Customer Customer { get; set; }

		public int IdOrderNote { get; set; }

		public OrderNote OrderNote { get; set; }
	}
}
