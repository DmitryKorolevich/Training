using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.Products
{
    public class ProductCategoryLite
    {
	    public string Label { get; set; }

	    public string Link { get; set; }

	    public IList<ProductCategoryLite> SubItems { get; set; }

	    public ProductCategoryLite()
	    {
			SubItems = new List<ProductCategoryLite>();
	    }
    }
}
