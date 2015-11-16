using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductCategoryLite
    {
        public int Id { get; set; }

        public string NavLabel { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }

    public class ProductNavCategoryLite : ProductCategoryLite
    {
        public IList<ProductNavCategoryLite> SubItems { get; set; }

        public ProductCategory ProductCategory { get; set; }

        public ProductNavCategoryLite()
	    {
			SubItems = new List<ProductNavCategoryLite>();
	    }
    }
}
