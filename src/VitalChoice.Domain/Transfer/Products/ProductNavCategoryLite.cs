using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Transfer.Products
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
