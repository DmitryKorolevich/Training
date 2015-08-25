﻿using System.Collections.Generic;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class VProductSkuFilter : FilterBase
    {
        public string Code { get; set; }

        public string DescriptionName { get; set; }

        public string ExactCode { get; set; }

        public string ExactDescriptionName { get; set; }

        public IList<int> Ids { get; set; }

        public IList<int> IdProducts { get; set; }
    }
}