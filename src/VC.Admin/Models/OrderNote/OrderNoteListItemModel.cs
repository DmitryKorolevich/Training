using System;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.OrderNote
{
    public class OrderNoteListItemModel : Model<VitalChoice.Domain.Entities.eCommerce.Orders.OrderNote, AbstractModeContainer<IMode>>
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

	    public IList<int> CustomerTypes { get; set; }

		public string EditedBy { get; set; }

		public DateTime DateEdited { get; set; }
	}
}