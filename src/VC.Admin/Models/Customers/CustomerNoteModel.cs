using System;
using VC.Admin.Validators.Customer;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

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