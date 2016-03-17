using System;
using VC.Admin.Validators.InventorySku;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.InventorySkus
{
    [ApiValidator(typeof(InventorySkuManageModelValidator))]
    public class InventorySkuManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public string Code { get; set; }

        [Map]
        public string Description { get; set; }

        [Map]
        public int? IdInventorySkuCategory { get; set; }
        
        [Map]
        public int? ProductSource { get; set; }

        [Map]
        public int? Quantity { get; set; }

        [Map]
        public int? UnitOfMeasure { get; set; }

        [Map]
        public decimal? UnitOfMeasureAmount { get; set; }

        [Map]
        public int? PurchaseUnitOfMeasure { get; set; }

        [Map]
        public int? PurchaseUnitOfMeasureAmount { get; set; }


        public InventorySkuManageModel()
        {

        }
    }
}