using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles
{
    public class TtlArticleCategoryModel
    {
	    public TtlArticleCategoryModel()
	    {
			SubCategories = new List<TtlArticleCategoryModel>();
        }

	    public string Name { get; set; }

		public string Url { get; set; }

	    public IList<TtlArticleCategoryModel> SubCategories { get; set; }
    }
}
