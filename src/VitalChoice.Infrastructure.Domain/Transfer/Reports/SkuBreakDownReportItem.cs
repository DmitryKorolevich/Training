namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuBreakDownReportItem
    {
        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public decimal RetailAmount { get; set; }

        public decimal WholesaleAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public int RetailQuantity { get; set; }

        public int WholesaleQuantity { get; set; }

        public int TotalQuantity { get; set; }

        public decimal RetailPercent { get; set; }

        public decimal WholesalePercent { get; set; }
    }
}