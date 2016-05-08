using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Emails
{
    public class EmailTemplate : ContentDataItem
    {
        public string ModelType { get; set; }

        public string EmailDescription { get; set; }

        public ICollection<string> ModelPropertyNames { get; set; }
    }
}