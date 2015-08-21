using VC.Admin.Validators.ContentManagement;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
	[ApiValidator(typeof(RecipeVideoModelValidator))]
	public class VideoRecipeModel: DefaultModeRecipeItem
	{
		public string Image { get; set; }

		public string Video { get; set; }

		public string Text { get; set; }
	}
}
