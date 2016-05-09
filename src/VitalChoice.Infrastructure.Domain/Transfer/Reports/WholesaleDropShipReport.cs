using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class WholesaleDropShipReport
    {
        public WholesaleDropShipReport()
        {
            Skus = new List<WholesaleDropShipReportSkuSummary>();
            SkusTotal =new WholesaleDropShipReportSkuSummary();
        }

        public decimal DiscountedSubtotal { get; set; }

        public decimal Shipping { get; set; }

        public decimal Total { get; set; }

        public ICollection<WholesaleDropShipReportSkuSummary> Skus { get; set; }

        public WholesaleDropShipReportSkuSummary SkusTotal { get; set; }
    }
}