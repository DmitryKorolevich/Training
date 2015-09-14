using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class ProductCategoryLiteFilter:FilterBase
    {
	    public IList<CustomerTypeCode> Visibility { get; set; }
    }
}
