﻿using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class VProductSkuFilter : FilterBase
    {
        public string Code { get; set; }

        public string DescriptionName { get; set; }

        public string ExactCode { get; set; }

        public ICollection<string> ExactCodes { get; set; }

        public string ExactDescriptionName { get; set; }

        public IList<int> Ids { get; set; }

        public IList<int> IdProducts { get; set; }

        public IList<ProductType> IdProductTypes { get; set; }

        public bool ActiveOnly { get; set; }

	    public bool NotHiddenOnly { get; set; }
    }
}