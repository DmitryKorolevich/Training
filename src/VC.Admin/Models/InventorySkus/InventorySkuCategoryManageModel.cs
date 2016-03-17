using VC.Admin.Validators.InventorySku;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.InventorySkus
{
    [ApiValidator(typeof(InventorySkuCategoryManageModelValidator))]
    public class InventorySkuCategoryManageModel : BaseModel
    {
        public int Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public InventorySkuCategoryManageModel()
        {
        }

        public InventorySkuCategoryManageModel(InventorySkuCategory item)
        {
            Id = item.Id;
            Name = item.Name;
            ParentId = item.ParentId;
        }

        public InventorySkuCategory Convert()
        {
            InventorySkuCategory toReturn = new InventorySkuCategory();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.ParentId = ParentId;

            return toReturn;
        }
    }
}