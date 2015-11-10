﻿using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Affiliates
{
    public class AffiliatesSummaryModel
    {
        public int AllAffiliates { get; set; }

        public int EngagedAffiliates { get; set; }

        public decimal EngagedPercent { get; set; }

        public int AffiliateCustomers { get; set; }
    }
}