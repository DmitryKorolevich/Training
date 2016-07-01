using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class OrderSkuCountReportOrderItem
    {
        public int IdOrder { get; set; }

        public int SkuCount { get; set; }

        public int CountOfGivenSku { get; set; }

        public decimal PercentOfTotal { get; set; }
    }
}