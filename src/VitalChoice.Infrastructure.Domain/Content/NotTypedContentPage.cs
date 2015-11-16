using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content
{
    public class NotTypedContentPage : Entity
    {
        public string Name { get; set; }

        public string Url { get; set; }
        
        public MasterContentItem MasterContentItem { get; set; }

        public ContentItem ContentItem { get; set; }

        public RecordStatusCode StatusCode { get; set; }
    }
}