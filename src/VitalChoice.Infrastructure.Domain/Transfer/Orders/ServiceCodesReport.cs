using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ServiceCodesReport
    {
        public IList<ServiceCodesReportItem> Items { get; set; }

        public int OrdersCount { get; set; }    

        public decimal OrdersAmount { get; set; }
    }
}
