using VC.Admin.Validators.ContentManagement;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.ContentManagement
{
	[ApiValidator(typeof(RelatedRecipeModelValidator))]
	public class RelatedRecipeModel:DefaultModeRecipeItem
	{
		public string Image { get; set; }

		public string Url { get; set; }

		public string Title { get; set; }
	}
}
