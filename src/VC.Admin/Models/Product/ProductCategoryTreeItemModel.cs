using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Product;

namespace VC.Admin.Models.Product
{
    public class ProductCategoryTreeItemModel : Model<ProductCategory, IMode>
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }
                
        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public IEnumerable<ProductCategoryTreeItemModel> SubItems { get; set; }

        public ProductCategoryTreeItemModel(ProductCategory item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                Url = item.Url;
                StatusCode = item.StatusCode;
                Assigned = item.Assigned;
                CreateSubCategories(this, item);
            }
        }

        private void CreateSubCategories(ProductCategoryTreeItemModel model, ProductCategory category)
        {
            var subModels = new List<ProductCategoryTreeItemModel>();
            foreach(var subCategory in category.SubCategories)
            {
                ProductCategoryTreeItemModel subModel = new ProductCategoryTreeItemModel(subCategory);
                subModels.Add(subModel);
            }
            model.SubItems = subModels;
        }

        public override ProductCategory Convert()
        {
            return Convert(null);
        }

        public ProductCategory Convert(int? parentId)
        {
            ProductCategory toReturn = new ProductCategory();
            toReturn.Id=Id;
            toReturn.ParentId = parentId;
            toReturn.Name = Name;
            toReturn.Url = Url;
            var subItems = new List<ProductCategory>();
            if (SubItems!=null)
            {
                foreach(var subItem in SubItems)
                {
                    subItems.Add(subItem.Convert(toReturn.Id));
                }
            }
            toReturn.SubCategories = subItems;
            return toReturn;
        }
    }
}