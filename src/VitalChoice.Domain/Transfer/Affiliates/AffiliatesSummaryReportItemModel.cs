using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Affiliates
{
    public class AffiliatesSummaryReportItemModel
    {
        public string Month { get; set; }

        public int NewTransactions { get; set; }

        public int RepeatTransactions { get; set; }

        public decimal NewTransactionsPercent { get; set; }

        public int TotalTransactions { get; set; }

        public decimal NewSales { get; set; }

        public decimal RepeatSales { get; set; }

        public decimal NewSalesPercent { get; set; }

        public decimal TotalSales { get; set; }
    }
}