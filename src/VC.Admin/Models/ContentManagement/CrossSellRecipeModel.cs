using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.ContentManagement
{
	[ApiValidator(typeof(CrossSellRecipeModelValidator))]
	public class CrossSellRecipeModel: RelatedRecipeModel
	{
		public string Subtitle { get; set; }
	}
}
