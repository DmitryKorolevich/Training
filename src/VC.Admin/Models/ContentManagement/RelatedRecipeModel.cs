using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Validators.ContentManagement;
using VC.Admin.Validators.Customer;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
	[ApiValidator(typeof(RelatedRecipeModelValidator))]
	public class RelatedRecipeModel:BaseModel
	{
		public string Image { get; set; }

		public string Url { get; set; }

		public string Title { get; set; }
	}
}
