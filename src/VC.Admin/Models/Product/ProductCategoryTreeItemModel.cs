using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VC.Admin.Models.Product
{
    public class ProductCategoryTreeItemModel : BaseModel
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }
                
        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public IEnumerable<ProductCategoryTreeItemModel> SubItems { get; set; }

        public ProductCategoryTreeItemModel(ProductCategory item)
        {
            if (item != null)
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
            var subModels = category.SubCategories.Select(subCategory => new ProductCategoryTreeItemModel(subCategory)).ToList();
            model.SubItems = subModels;
        }

        public ProductCategory Convert()
        {
            return Convert(null);
        }

        public ProductCategory Convert(int? parentId)
        {
            ProductCategory toReturn = new ProductCategory
            {
                Id = Id,
                ParentId = parentId,
                Name = Name,
                Url = Url
            };
            var subItems = new List<ProductCategory>();
            if (SubItems!=null)
            {
                subItems.AddRange(SubItems.Select(subItem => subItem.Convert(toReturn.Id)));
            }
            toReturn.SubCategories = subItems;
            return toReturn;
        }
    }
}