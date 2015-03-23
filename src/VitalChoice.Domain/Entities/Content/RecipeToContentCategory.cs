using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class RecipeToContentCategory : Entity
    {
        public Recipe Recipe { get; set; }

        public int RecipeId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}