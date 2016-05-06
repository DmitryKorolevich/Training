using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class WholesaleDropShipReportSkuItem
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}