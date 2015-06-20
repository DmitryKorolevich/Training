using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public class OrderNoteToCustomerType: Entity
    {
	    public OrderNote OrderNote { get; set; }

	    public CustomerTypeEntity CustomerType { get; set; }

	    public int IdOrderNote { get; set; }

	    public int IdCustomerType { get; set; }
    }
}
