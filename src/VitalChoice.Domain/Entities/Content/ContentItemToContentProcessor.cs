using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentItemToContentProcessor : Entity
    {
        public ContentItem ContentItem { get; set; }
        public int ContentItemId { get; set; }

        public ContentProcessorEntity ContentProcessor { get; set; }
        public int ContentItemProcessorId { get; set; }
    }
}