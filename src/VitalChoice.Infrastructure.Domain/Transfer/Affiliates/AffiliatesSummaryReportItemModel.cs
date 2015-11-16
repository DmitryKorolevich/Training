namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
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