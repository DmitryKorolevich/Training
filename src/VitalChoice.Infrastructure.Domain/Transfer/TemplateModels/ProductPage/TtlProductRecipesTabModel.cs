using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductRecipesTabModel: TtlProductPageTabModel
    {
	    public TtlProductRecipesTabModel()
	    {
		    Recipes = new List<TtlProductRecipeModel>();
	    }

	    public IList<TtlProductRecipeModel> Recipes { get; set; }
    }
}
