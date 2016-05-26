using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class WholesaleSummaryReportMonthStatistic
    {
        public string Month { get; set; }

        public decimal EstablishedSales { get; set; }

        public decimal NewSales { get; set; }

        public decimal Total { get; set; }
    }
}
