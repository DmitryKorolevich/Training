using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerNote: DynamicDataEntity<CustomerNoteOptionValue, CustomerNoteOptionType>
	{
	    public int IdCustomer { get; set; }

	    public string Note { get; set; }
	}
}
