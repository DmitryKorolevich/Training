using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.ContentPages
{
    public class ContentPage : ContentDataItem
    {
        public CustomerTypeCode Assigned { get; set; }

        public virtual ICollection<ContentPageToContentCategory> ContentPagesToContentCategories { get; set; }
    }
}