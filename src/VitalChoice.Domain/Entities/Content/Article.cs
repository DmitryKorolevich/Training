﻿using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
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