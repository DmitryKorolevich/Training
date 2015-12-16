using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes
{
    public class TtlRecipeCategoriesModel
    {
        public TtlRecipeCategoriesModel()
        {
            AllCategories = new List<TtlRecipeCategoryModel>();
            SubCategories = new List<TtlRecipeCategoryModel>();
            ChefCategories = new List<TtlRecipeCategoryModel>();
        }

        public ICollection<TtlRecipeCategoryModel> AllCategories { get; set; }

        public ICollection<TtlRecipeCategoryModel> ChefCategories { get; set; }

        public ICollection<TtlRecipeCategoryModel> SubCategories { get; set; }
    }
}
