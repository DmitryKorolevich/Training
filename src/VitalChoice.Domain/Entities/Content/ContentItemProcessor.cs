using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentItemProcessor : Entity
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public IEnumerable<ContentItemToContentItemProcessor> ContentItemToContentItemProcessors { get; set; }
    }
}