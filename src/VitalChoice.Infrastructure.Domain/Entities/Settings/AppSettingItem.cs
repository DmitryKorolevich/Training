using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Settings
{
    public class AppSettingItem : Entity
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
