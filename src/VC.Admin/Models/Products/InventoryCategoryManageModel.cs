using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
{
    [ApiValidator(typeof(InventoryCategoryManageModelValidator))]
    public class InventoryCategoryManageModel : BaseModel
    {
        public int Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public InventoryCategoryManageModel()
        {
        }

        public InventoryCategoryManageModel(InventoryCategory item)
        {
            Id = item.Id;
            Name = item.Name;
            ParentId = item.ParentId;
        }

        public InventoryCategory Convert()
        {
            InventoryCategory toReturn = new InventoryCategory();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.ParentId = ParentId;

            return toReturn;
        }
    }
}