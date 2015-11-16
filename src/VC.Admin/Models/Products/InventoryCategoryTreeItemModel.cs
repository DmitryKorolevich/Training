using System.Linq;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Product
{
    public class InventoryCategoryTreeItemModel : BaseModel
	{
	    public int Id { get; set; }

	    public string Name { get; set; }
                
        public RecordStatusCode StatusCode { get; set; }

        public IList<InventoryCategoryTreeItemModel> SubItems { get; set; }

        public InventoryCategoryTreeItemModel(InventoryCategory item)
        {
            if (item != null)
            {
                Id = item.Id;
                Name = item.Name;
                StatusCode = item.StatusCode;
                CreateSubCategories(this, item);
            }
        }

        private void CreateSubCategories(InventoryCategoryTreeItemModel model, InventoryCategory category)
        {
            var subModels = category.SubCategories.Select(subCategory => new InventoryCategoryTreeItemModel(subCategory)).ToList();
            model.SubItems = subModels;
        }

        public InventoryCategory Convert()
        {
            return Convert(null);
        }

        public InventoryCategory Convert(int? parentId)
        {
            InventoryCategory toReturn = new InventoryCategory
            {
                Id = Id,
                ParentId = parentId,
                Name = Name,
            };
            var subItems = new List<InventoryCategory>();
            if (SubItems!=null)
            {
                subItems.AddRange(SubItems.Select(subItem => subItem.Convert(toReturn.Id)));
            }
            toReturn.SubCategories = subItems;
            return toReturn;
        }
    }
}