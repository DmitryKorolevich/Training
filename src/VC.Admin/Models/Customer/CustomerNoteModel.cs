using System;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
	[ApiValidator(typeof(CustomerNoteModelValidator))]
	public class CustomerNoteModel : BaseModel
	{
		[Map]
		public int Id { get; set; }

		[Map]
	    public CustomerNotePriority Priority { get; set; }

		[Map("Note")]
	    public string Text { get; set; }

		[Map]
		public DateTime DateEdited { get; set; }

		public string EditedBy { get; set; }
	}
}