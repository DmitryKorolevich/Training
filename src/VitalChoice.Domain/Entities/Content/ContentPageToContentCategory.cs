using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentPageToContentCategory : Entity
    {
        public ContentPage ContentPage { get; set; }

        public int ContentPageId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}