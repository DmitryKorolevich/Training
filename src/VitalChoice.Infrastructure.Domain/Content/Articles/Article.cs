using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Articles
{
    public class Article : ContentDataItem
    {
        public string FileUrl { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string SubTitle { get; set; }

        public string Author { get; set; }

        public virtual ICollection<ArticleToContentCategory> ArticlesToContentCategories { get; set; }

        public virtual ICollection<ArticleToProduct> ArticlesToProducts { get; set; }
    }
}