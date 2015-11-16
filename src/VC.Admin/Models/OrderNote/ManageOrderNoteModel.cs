using System.Collections.Generic;
using VC.Admin.Validators.OrderNote;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.OrderNote
{
	[ApiValidator(typeof(OrderNoteManageValidator))]
	public class ManageOrderNoteModel : BaseModel<AbstractModeContainer<IMode>>
	{
		public int Id { get; set; }

		[Localized(GeneralFieldNames.Label)]
		public string Title { get; set; }

		[Localized(GeneralFieldNames.OrderNote)]
		public string Description { get; set; }

		public IList<int> CustomerTypes { get; set; }
	}
}