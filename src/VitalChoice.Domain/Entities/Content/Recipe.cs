using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class Recipe : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }
        public MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }


        public RecordStatusCode StatusCode { get; set; }

        public virtual ICollection<RecipeToContentCategory> RecipesToContentCategories { get; set; }
    }
}