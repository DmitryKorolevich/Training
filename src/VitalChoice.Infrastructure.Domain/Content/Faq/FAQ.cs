using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Faq
{
    public class FAQ : ContentDataItem
    {
        public virtual ICollection<FAQToContentCategory> FAQsToContentCategories { get; set; }
    }
}