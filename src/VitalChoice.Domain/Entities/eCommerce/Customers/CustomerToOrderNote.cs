namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerToOrderNote : Entity
	{
		public int IdCustomer { get; set; }

		public int IdOrderNote { get; set; }
	}
}
