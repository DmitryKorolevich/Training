using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.FAQs
{
    public class TtlFAQCategoriesModel
    {
        public TtlFAQCategoriesModel()
        {
            AllCategories = new List<TtlFAQCategoryModel>();
            SubCategories = new List<TtlFAQCategoryModel>();
        }

        public ICollection<TtlFAQCategoryModel> AllCategories { get; set; }

        public ICollection<TtlFAQCategoryModel> SubCategories { get; set; }
    }
}
