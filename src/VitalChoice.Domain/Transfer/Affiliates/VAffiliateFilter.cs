using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Affiliates
{
    public class VAffiliateFilter : FilterBase
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public int? Tier { get; set; }
    }
}