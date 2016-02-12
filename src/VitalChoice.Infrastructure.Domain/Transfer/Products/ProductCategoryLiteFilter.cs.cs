using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductCategoryLiteFilter: ProductCategoryTreeFilter
    {
	    public IList<CustomerTypeCode> Visibility { get; set; }

        //Show all not deleted categories
        public bool ShowAll { get; set; }
	}
}
