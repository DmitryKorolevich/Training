using System.Linq;
using System.Collections.Generic;
using VC.Admin.Models.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VC.Admin.ModelConverters
{
    public class SkuModelConverter : BaseModelConverter<SKUManageModel, SkuDynamic>
    {
        private readonly IInventorySkuService _inventorySkuService;

        public SkuModelConverter(IInventorySkuService inventorySkuService)
        {
            _inventorySkuService = inventorySkuService;
        }

        public override void DynamicToModel(SKUManageModel model, SkuDynamic dynamic)
        {
            model.Active = dynamic.StatusCode == (int)RecordStatusCode.Active;
            model.InventorySkus=new List<InventorySkuListItemModel>();
            if (dynamic.SkusToInventorySkus != null && dynamic.SkusToInventorySkus.Count != 0)
            {
                InventorySkuFilter filter = new InventorySkuFilter();
                filter.Ids = dynamic.SkusToInventorySkus.Select(p => p.IdInventorySku).ToList();
                var items = _inventorySkuService.GetInventorySkusAsync(filter).Result.Items;
                model.InventorySkus = items;
                foreach (var item in model.InventorySkus)
                {
                    var reference = dynamic.SkusToInventorySkus.FirstOrDefault(p => p.IdInventorySku == item.Id);
                    if(reference!=null)
                    {
                        item.Quantity = reference.Quantity;
                    }
                }
            }
        }

        public override void ModelToDynamic(SKUManageModel model, SkuDynamic dynamic)
        {
            dynamic.StatusCode = model.Active ? (int)RecordStatusCode.Active : (int)RecordStatusCode.NotActive;
            dynamic.Code = dynamic?.Code.Trim();
            if (!dynamic.Data.AutoShipProduct)
            {
                dynamic.DictionaryData.Remove("AutoShipFrequency1");
                dynamic.DictionaryData.Remove("AutoShipFrequency2");
                dynamic.DictionaryData.Remove("AutoShipFrequency3");
                dynamic.DictionaryData.Remove("AutoShipFrequency6");
                dynamic.DictionaryData.Remove("OffPercent");
            }

            dynamic.SkusToInventorySkus = model.InventorySkus?.Select(p => new SkuToInventorySku() {
                IdInventorySku=p.Id,
                Quantity=p.Quantity ?? 1,
            }).ToList();
        }
    }
}