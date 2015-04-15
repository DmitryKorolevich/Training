using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ArticleToContentCategory : Entity
    {
        public Article Article { get; set; }

        public int ArticleId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}