using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.Products
{
    public class ProductNavCategoryLite
    {
	    public string Label { get; set; }

	    public string Link { get; set; }

	    public IList<ProductNavCategoryLite> SubItems { get; set; }

	    public ProductNavCategoryLite()
	    {
			SubItems = new List<ProductNavCategoryLite>();
	    }
    }
}
