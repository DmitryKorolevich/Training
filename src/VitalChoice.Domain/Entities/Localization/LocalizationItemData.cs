using System;
using VitalChoice.Domain;

namespace VitalChoice.Domain.Entities.Localization
{
	public class LocalizationItemData : Entity
    {
        public int GroupId { get; set; }

        public int ItemId { get; set; }

        public string CultureId { get; set; }

        public string Value { get; set; }
    }
}
