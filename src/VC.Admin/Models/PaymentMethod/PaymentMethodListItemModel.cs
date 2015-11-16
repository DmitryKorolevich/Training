using System;
using System.Collections.Generic;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.PaymentMethod
{
    public class PaymentMethodListItemModel: BaseModel
	{
	    public PaymentMethodListItemModel()
	    {
			CustomerTypes = new List<int>();
        }

	    public int Id { get; set; }

	    public string Name { get; set; }

		public DateTime DateEdited { get; set; }

		public string EditedBy { get; set; }

		public IList<int> CustomerTypes { get; set; }

	}
}
