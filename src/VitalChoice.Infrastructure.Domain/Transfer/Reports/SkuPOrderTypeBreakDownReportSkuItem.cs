using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuPOrderTypeBreakDownReportSkuItem
    {
        public SkuPOrderTypeBreakDownReportSkuItem()
        {
            Periods = new List<SkuPOrderTypeBreakDownReportSkuPeriod>();
        }

        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public IList<SkuPOrderTypeBreakDownReportSkuPeriod> Periods { get; set; }
    }
}