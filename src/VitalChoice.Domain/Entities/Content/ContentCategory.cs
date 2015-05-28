using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentCategory : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string FileUrl { get; set; }        

        public int? ParentId { get; set; }

        public ContentCategory Parent { get; set; }

        public IEnumerable<ContentCategory> SubCategories { get; set; }

        public virtual MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public virtual ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }

        public int Order { get; set; }

        public ContentType Type { get; set; }

        public RecordStatusCode StatusCode { get; set; }
    }
}