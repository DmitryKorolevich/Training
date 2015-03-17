using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentItem : Entity
    {
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public IEnumerable<ContentItemToContentItemProcessor> ContentItemToContentItemProcessors { get; set; }
    }
}