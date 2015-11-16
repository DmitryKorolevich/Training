using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Localization
{
	public class LocalizationItemData : Entity
    {
        public int GroupId { get; set; }

        public int ItemId { get; set; }

        public string CultureId { get; set; }

        public string Value { get; set; }

        public LocalizationItem LocalizationItem { get; set; }
    }
}
