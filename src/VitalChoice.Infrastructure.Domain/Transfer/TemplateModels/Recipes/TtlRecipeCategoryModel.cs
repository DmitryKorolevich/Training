using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes
{
    public class TtlRecipeCategoryModel
    {
	    public TtlRecipeCategoryModel()
	    {
			SubCategories = new List<TtlRecipeCategoryModel>();
            Recipes = new List<TtlShortRecipeModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

		public string Url { get; set; }

	    public IList<TtlRecipeCategoryModel> SubCategories { get; set; }

        public IList<TtlShortRecipeModel> Recipes { get; set; }
    }
}
