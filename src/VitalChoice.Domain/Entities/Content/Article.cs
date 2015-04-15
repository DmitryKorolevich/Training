using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class Article : ContentDataItem
    {
        public DateTime? PublishedDate { get; set; }

        public string SubTitle { get; set; }

        public string Author { get; set; }

        public virtual ICollection<ArticleToContentCategory> ArticlesToContentCategories { get; set; }
    }
}