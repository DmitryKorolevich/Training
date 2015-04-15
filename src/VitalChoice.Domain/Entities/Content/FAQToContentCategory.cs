using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class FAQToContentCategory : Entity
    {
        public FAQ FAQ { get; set; }

        public int FAQId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}