using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.InventorySkus
{
    public class InventorySkuCategoryTreeItemModel : BaseModel
	{
	    public int Id { get; set; }

	    public string Name { get; set; }
                
        public RecordStatusCode StatusCode { get; set; }

        public IList<InventorySkuCategoryTreeItemModel> SubItems { get; set; }

        public InventorySkuCategoryTreeItemModel(InventorySkuCategory item)
        {
            if (item != null)
            {
                Id = item.Id;
                Name = item.Name;
                StatusCode = item.StatusCode;
                CreateSubCategories(this, item);
            }
        }

        private void CreateSubCategories(InventorySkuCategoryTreeItemModel model, InventorySkuCategory category)
        {
            var subModels = category.SubCategories.Select(subCategory => new InventorySkuCategoryTreeItemModel(subCategory)).ToList();
            model.SubItems = subModels;
        }

        public InventorySkuCategory Convert()
        {
            return Convert(null);
        }

        public InventorySkuCategory Convert(int? parentId)
        {
            InventorySkuCategory toReturn = new InventorySkuCategory
            {
                Id = Id,
                ParentId = parentId,
                Name = Name,
            };
            var subItems = new List<InventorySkuCategory>();
            if (SubItems!=null)
            {
                subItems.AddRange(SubItems.Select(subItem => subItem.Convert(toReturn.Id)));
            }
            toReturn.SubCategories = subItems;
            return toReturn;
        }
    }
}