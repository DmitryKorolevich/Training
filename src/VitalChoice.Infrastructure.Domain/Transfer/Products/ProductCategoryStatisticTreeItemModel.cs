using System.Linq;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductCategoryStatisticTreeItemModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
                
        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public IEnumerable<ProductCategoryStatisticTreeItemModel> SubItems { get; set; }

        public decimal Amount { get; set; }

        public decimal Percent { get; set; }

        public ProductCategoryStatisticTreeItemModel(ProductCategory item)
        {
            if (item != null)
            {
                Id = item.Id;
                Name = item.Name;
                StatusCode = item.StatusCode;
                Assigned = item.Assigned;
                CreateSubCategories(this, item);
            }
        }

        private void CreateSubCategories(ProductCategoryStatisticTreeItemModel model, ProductCategory category)
        {
            model.SubItems =
                category.SubCategories.Select(subCategory => new ProductCategoryStatisticTreeItemModel(subCategory)).ToList();
        }
	}
}