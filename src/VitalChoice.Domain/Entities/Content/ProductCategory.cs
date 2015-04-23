using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ProductCategory : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string FileImageSmallUrl { get; set; }

        public string FileImageLargeUrl { get; set; }

        public string LongDescription { get; set; }

        public string LongDescriptionBottom { get; set; }

        public int? ParentId { get; set; }

        public ProductCategory Parent { get; set; }

        public IEnumerable<ProductCategory> SubCategories { get; set; }

        public virtual MasterContentItem MasterContentItem { get; set; }

        public int MasterContentItemId { get; set; }

        public virtual ContentItem ContentItem { get; set; }

        public int ContentItemId { get; set; }

        public int Order { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }
    }
}