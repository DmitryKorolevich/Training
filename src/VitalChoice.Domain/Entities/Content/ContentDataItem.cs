using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.Content
{
    public abstract class ContentDataItem : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }
        
        public MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int? UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}