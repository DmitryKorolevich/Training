using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class ContentCategory : ContentDataItem
    {
        public string FileUrl { get; set; }

        public int? ParentId { get; set; }

        public ContentCategory Parent { get; set; }

        public ICollection<ContentCategory> SubCategories { get; set; }

        public int Order { get; set; }

        public ContentType Type { get; set; }
    }
}