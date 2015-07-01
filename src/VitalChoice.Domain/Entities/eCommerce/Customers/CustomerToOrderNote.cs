using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerToOrderNote : Entity
	{
		public int IdCustomer { get; set; }

		public Customer Customer { get; set; }

		public int IdOrderNote { get; set; }

		public OrderNote OrderNote { get; set; }
	}
}
