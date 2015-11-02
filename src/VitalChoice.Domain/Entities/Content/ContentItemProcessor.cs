using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentProcessorEntity : Entity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ContentItemToContentProcessor> ContentItemsToContentProcessors { get; set; }
        public virtual ICollection<MasterContentItemToContentProcessor> MasterContentItemsToContentProcessors { get; set; }
    }
}