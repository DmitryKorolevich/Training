using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class ContentCategory : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string FileUrl { get; set; }        

        public int? ParentId { get; set; }

        public ContentCategory Parent { get; set; }

        public ICollection<ContentCategory> SubCategories { get; set; }

        public virtual MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public virtual ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }

        public int Order { get; set; }

        public ContentType Type { get; set; }

        public RecordStatusCode StatusCode { get; set; }
    }
}