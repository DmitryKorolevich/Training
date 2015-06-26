using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class Recipe : ContentDataItem
    {
        public virtual ICollection<RecipeToContentCategory> RecipesToContentCategories { get; set; }

        public virtual ICollection<RecipeToProduct> RecipesToProducts { get; set; }

    }
}