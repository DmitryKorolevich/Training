using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Customers
{
    public class CustomerNote: DynamicDataEntity<CustomerNoteOptionValue, CustomerNoteOptionType>
	{
	    public int IdCustomer { get; set; }

	    public string Note { get; set; }

        public int? IdAddedBy { get; set; }
        
        public User AddedBy { get; set; }
    }
}
