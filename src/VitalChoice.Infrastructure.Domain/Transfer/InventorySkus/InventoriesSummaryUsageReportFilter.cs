﻿using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventoriesSummaryUsageReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public bool ShipDate { get; set; }

        public string Sku { get; set; }

        public string InvSku { get; set; }

        public bool? Assemble { get; set; }

        public IList<int> IdsInvCat { get; set; }

        public int InfoType { get; set; }

        public FrequencyType FrequencyType { get; set; }
    }
}