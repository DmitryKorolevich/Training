using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles
{
    public class TtlArticleCategoriesModel
    {
	    public ICollection<TtlArticleCategoryModel> Categories { get; set; }

        public string ShowAllLink { get; set; }
    }
}
