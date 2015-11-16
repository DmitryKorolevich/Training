using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class ContentTypeEntity : Entity
    {
        public string Name { get; set; }

        public int? DefaultMasterContentItemId { get; set; }
    }
}