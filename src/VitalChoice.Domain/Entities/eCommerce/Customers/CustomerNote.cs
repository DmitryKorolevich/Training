using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerNote: DynamicDataEntity<CustomerOptionValue, CustomerOptionType>
	{
	    public int IdCustomer { get; set; }

	    public string Note { get; set; }
	}
}
