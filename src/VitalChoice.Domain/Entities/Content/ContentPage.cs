using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentPage : ContentDataItem
    {
        public CustomerTypeCode Assigned { get; set; }

        public virtual ICollection<ContentPageToContentCategory> ContentPagesToContentCategories { get; set; }
    }
}