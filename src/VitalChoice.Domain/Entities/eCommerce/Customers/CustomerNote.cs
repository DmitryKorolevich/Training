using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerNote: DynamicDataEntity<CustomerNoteOptionValue, CustomerNoteOptionType>
	{
	    public int IdCustomer { get; set; }

	    public string Note { get; set; }
	}
}
