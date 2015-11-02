using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;

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

        public ProductCategoryTreeItemModel(ProductNavCategoryLite item)
        {
            if (item != null)
            {
                Id = item.Id;
                Url = item.Url;
                if (item.ProductCategory != null)
                {
                    Name = item.ProductCategory.Name;
                    StatusCode = item.ProductCategory.StatusCode;
                    Assigned = item.ProductCategory.Assigned;
                }
                CreateSubCategories(this, item);
            }
        }

        private void CreateSubCategories(ProductCategoryTreeItemModel model, ProductNavCategoryLite category)
        {
            model.SubItems =
                category.SubItems.Select(subCategory => new ProductCategoryTreeItemModel(subCategory)).ToList();
        }

        public ProductNavCategoryLite Convert()
        {
            return Convert(null);
        }

        public ProductNavCategoryLite Convert(int? parentId)
        {
            ProductNavCategoryLite toReturn = new ProductNavCategoryLite()
            {
                Id = Id,
                ProductCategory = new ProductCategory()
                {
                    ParentId = parentId
                },
                Name = Name,
                Url = Url
            };
            var subItems = new List<ProductNavCategoryLite>();
            if (SubItems != null)
            {
                subItems.AddRange(SubItems.Select(subItem => subItem.Convert(toReturn.Id)));
            }
            toReturn.SubItems = subItems.ToList();
            return toReturn;
        }
	}
}