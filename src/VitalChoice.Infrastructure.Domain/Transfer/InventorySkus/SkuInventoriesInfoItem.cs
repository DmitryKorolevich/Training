using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class SkuInventoryInfoItem
    {
        public string Code { get; set; }

        public int IdInventorySku { get; set; }

        public int Quantity { get; set; }
    }

    public class SkuInventoriesInfoItem
    {
        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public RecordStatusCode ProductStatusCode { get; set; }

        public CustomerTypeCode? ProductIdVisibility { get; set; }

        public IList<SkuInventoryInfoItem> Inventories { get; set; }

        public string InventoriesLine { get; set; }
    }
}