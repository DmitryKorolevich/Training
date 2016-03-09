using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
{
    public class SKUManageModel : BaseModel 
    {
        [Map]
        public int Id { get; set; }

        [Map("Code")]
        [Localized(GeneralFieldNames.SKU)]
        public string Name { get; set; }

        [Map]
        public int QTY { get; set; }

        public bool Active { get; set; }

        [Map]
        public bool Hidden { get; set; }

        [Map("Price")]
        public decimal RetailPrice { get; set; }

        [Map]
        public decimal WholesalePrice { get; set; }

        [Map]
        public int? Stock { get; set; }

        [Map]
        public bool DisregardStock { get; set; }

        [Map]
        public bool DisallowSingle { get; set; }

        [Map]
        public bool NonDiscountable { get; set; }

        //1
        [Map]
        public bool OrphanType { get; set; }

        //1
        [Map]
        public int? QTYThreshold { get; set; }

        [Map]
        public bool AutoShipProduct { get; set; }

        [Map]
        public double? OffPercent { get; set; }

        [Map]
        public int Seller { get; set; }

        [Map]
        public bool HideFromDataFeed { get; set; }

        [Map]
        public bool AutoShipFrequency1 { get; set; }

        [Map]
        public bool AutoShipFrequency2 { get; set; }

        [Map]
        public bool AutoShipFrequency3 { get; set; }

        [Map]
        public bool AutoShipFrequency6 { get; set; }

        [Map]
        public string SalesText { get; set; }

        [Map]
        public int? InventorySkuChannel { get; set; }

        [Map]
        public bool? Assemble { get; set; }

        public ICollection<InventorySkuListItemModel> InventorySkus { get; set; }
    }
}