using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Localization
{
    public class LocalizationItem : Entity
    {
        public int GroupId { get; set; }

        public int ItemId { get; set; }

        public string GroupName { get; set; }

        public string ItemName { get; set; }

        public string Comment { get; set; }

        public List<LocalizationItemData> LocalizationItemDatas { get;set;}
    }
}
