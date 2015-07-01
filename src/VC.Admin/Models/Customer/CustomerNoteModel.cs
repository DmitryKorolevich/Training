using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
	[ApiValidator(typeof(CustomerNoteModelValidator))]
	public class CustomerNoteModel : Model<CustomerNote, IMode>//, IModelToDynamic<AddressDynamic>
	{
		[Map]
	    public CustomerNotePriority Priority { get; set; }

		[Map]
	    public string Text { get; set; }
	}
}