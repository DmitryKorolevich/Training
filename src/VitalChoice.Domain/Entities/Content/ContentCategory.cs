using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentCategory : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public ContentCategory Parent { get; set; }

        public MasterContentItem MasterContentItem { get; set; }

        public ContentItem ContentItem { get; set; }

        public RecordStatusCode StatusCode { get; set; }
    }
}