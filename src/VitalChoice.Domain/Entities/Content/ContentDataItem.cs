using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentDataItem : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }
        
        public string FileUrl { get; set; }
        public MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public virtual ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int? UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}