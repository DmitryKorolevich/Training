using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
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