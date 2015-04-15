using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class FAQ : ContentDataItem
    {
        public virtual ICollection<FAQToContentCategory> FAQsToContentCategories { get; set; }
    }
}