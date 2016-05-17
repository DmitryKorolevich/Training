using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class WholesaleDropShipReportSkuRawItem : Entity
    { 
        public string Code { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal Total { get; set; }
    }
}
