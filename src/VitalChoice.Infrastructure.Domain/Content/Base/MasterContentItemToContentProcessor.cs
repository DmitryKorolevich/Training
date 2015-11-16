using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class MasterContentItemToContentProcessor : Entity
    {
        public MasterContentItem MasterContentItem { get; set; }
        public int MasterContentItemId { get; set; }

        public ContentProcessorEntity ContentProcessor { get; set; }
        public int ContentProcessorId { get; set; }
    }
}