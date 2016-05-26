using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class WholesaleSummaryReportTradeClassItem
    {
        public int Id { get; set; }

        public string Name { get; set; }    

        public int Count { get; set; }
    }
}
