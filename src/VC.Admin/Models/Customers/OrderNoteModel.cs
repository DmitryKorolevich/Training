﻿using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
    public class OrderNoteModel : BaseModel<AbstractModeContainer<IMode>>
	{
		public string Name { get; set; }

	    public int Id { get; set; }
	}
}