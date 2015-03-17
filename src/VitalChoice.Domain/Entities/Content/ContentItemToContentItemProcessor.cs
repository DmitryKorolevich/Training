using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentItemToContentItemProcessor : Entity
    {
        public ContentItem ContentItem { get; set; }
        public int ContentItemId { get; set; }

        public ContentItemProcessor ContentItemProcessor { get; set; }
        public int ContentItemProcessorId { get; set; }
    }
}