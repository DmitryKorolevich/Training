using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class ContentItemToContentProcessor : Entity
    {
        public ContentItem ContentItem { get; set; }
        public int ContentItemId { get; set; }

        public ContentProcessorEntity ContentProcessor { get; set; }
        public int ContentItemProcessorId { get; set; }
    }
}