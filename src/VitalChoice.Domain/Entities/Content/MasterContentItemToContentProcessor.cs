using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class MasterContentItemToContentProcessor : Entity
    {
        public MasterContentItem MasterContentItem { get; set; }
        public int MasterContentItemId { get; set; }

        public ContentProcessor ContentProcessor { get; set; }
        public int ContentProcessorId { get; set; }
    }
}