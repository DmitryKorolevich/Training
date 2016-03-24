using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ServiceCodesReportItem
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public int ReturnsCount { get; set; }

        public int RefundsCount { get; set; }

        public int ReshipsCount { get; set; }

        public int TotalCount { get; set; }

        public decimal ReturnsAmount { get; set; }

        public decimal RefundsAmount { get; set; }

        public decimal ReshipsAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal CountPercent { get; set; }

        public decimal AmountPercent { get; set; }
    }
}
