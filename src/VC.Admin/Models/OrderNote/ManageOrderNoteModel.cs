using System.Collections.Generic;
using VC.Admin.Validators.OrderNote;
using VC.Admin.Validators.UserManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.OrderNote
{
	[ApiValidator(typeof(OrderNoteManageValidator))]
	public class ManageOrderNoteModel : Model<VitalChoice.Domain.Entities.eCommerce.Orders.OrderNote, AbstractModeContainer<IMode>>
	{
		public int Id { get; set; }

		[Localized(GeneralFieldNames.Title)]
		public string Title { get; set; }

		[Localized(GeneralFieldNames.Description)]
		public string Description { get; set; }

		public IList<int> CustomerTypes { get; set; }
	}
}